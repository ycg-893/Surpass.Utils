using Surpass.Utils.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 匿名数据类型返回
    /// </summary>
    public class AnonymousDataResult
    {
        /// <summary>
        /// 实例化 AnonymousDataResult 类新实例
        /// </summary>
        public AnonymousDataResult()
        {
        }

        /// <summary>
        /// 新表达式头
        /// </summary>
        /// <param name="builder">生成器</param>
        /// <param name="body">主体</param>
        /// <returns></returns>
        private Expression NewExpressionBody(ObjectBuilder builder, NewExpression body)
        {
            List<Expression> projectionExpressions = new List<Expression>();
            int memberIndex = 0;
            foreach (var item in body.Members)
            {
                Expression Argument;
                Expression CurrentExpression = body.Arguments[memberIndex];
                Type type = item.GetMemberType();
                if (type.IsBaseTypeOrNullableDefinitionBaseTType()) //基本类型
                {
                    Argument = builder.ScalarExpressionBody(type, item.Name);
                }
                else if (type.IsClass && !type.IsEnumerableType()) //非枚举类型
                {
                    if (CurrentExpression is NewExpression && type.IsAnonymousType())
                    {
                        Argument = this.NewExpressionBody(builder, (NewExpression)CurrentExpression);
                    }
                    else
                    {
                        if (!type.IsDefaultConstructor())
                        {
                            throw new ArgumentException(string.Format(Resources.TypeNotDefaultConstructor, type.FullName));
                        }
                        Argument = builder.ObjectExpressionBody(type);
                    }
                }
                else if (CurrentExpression is NewExpression)
                {
                    Argument = this.NewExpressionBody(builder, (NewExpression)CurrentExpression);
                }
                else
                {
                    throw new ArgumentException(string.Format(Resources.NotSupportedType, type.FullName));
                }
                projectionExpressions.Add(Argument);
                memberIndex++;
            }
            return Expression.New(body.Constructor, projectionExpressions, body.Members);
        }

        /// <summary>
        /// 成员头
        /// </summary>
        /// <param name="builder">生成器</param>
        /// <param name="body">主体</param>
        /// <returns></returns>
        private Expression MemberExpressionBody(ObjectBuilder builder, MemberExpression body)
        {
            return builder.ScalarExpressionBody(body.Member.GetMemberType(), body.Member.Name);
        }

        /// <summary>
        /// 生成头
        /// </summary>
        /// <param name="builder">生成器</param>
        /// <param name="body">主体</param>
        /// <returns></returns>
        private Expression ExpressionBody(ObjectBuilder builder, Expression body)
        {
            if (body is NewExpression)
            {
                return this.NewExpressionBody(builder, (NewExpression)body);
            }
            else if (body is MemberExpression)
            {
                return this.MemberExpressionBody(builder, (MemberExpression)body);
            }
            else if (body is UnaryExpression)
            {
                UnaryExpression unary = (UnaryExpression)body;
                if (unary.Operand is MemberExpression)
                {
                    return this.MemberExpressionBody(builder, (MemberExpression)unary.Operand);
                }
            }
            throw new ArgumentException(string.Format(Resources.NotSupportedExpression, body.ToString()));
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">数据阅读器</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public Func<IDataResult, T> CreateInstanceDelegate<T>(IDataReader dataReader, Expression<Func<object, T>> selector)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            ExceptionUtils.CheckNotNull(selector, nameof(selector));
            try
            {
                Dictionary<string, int> columns = ObjectBuilder.BuilderColumns(dataReader);
                ObjectBuilder buider = new ObjectBuilder(columns);
                Expression expression = this.ExpressionBody(buider, selector.Body);
                return Expression.Lambda<Func<IDataResult, T>>(expression, ADataResult.RowParameter).Compile();
            }
            catch
            {
                dataReader.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">数据阅读器</param>
        /// <param name="resultDelegate">选择器</param>
        /// <returns></returns>
        public List<T> ToList<T>(IDataReader dataReader, Func<IDataResult, T> resultDelegate)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            ExceptionUtils.CheckNotNull(resultDelegate, nameof(resultDelegate));
            List<T> items = new List<T>();
            using (IDataReader reader = dataReader)
            {
                IDataResult result = new DataReaderResult(reader);
                while (reader.Read())
                {
                    items.Add(resultDelegate(result));
                }
            }
            return items;
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">数据阅读器</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public List<T> ToList<T>(IDataReader dataReader, Expression<Func<object, T>> selector)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            ExceptionUtils.CheckNotNull(selector, nameof(selector));
            Func<IDataResult, T> resultDelegate = this.CreateInstanceDelegate(dataReader, selector);
            return this.ToList<T>(dataReader, resultDelegate);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public Func<IDataResult, T> CreateInstanceDelegate<T>(DataTable dataTable, Expression<Func<object, T>> selector)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            ExceptionUtils.CheckNotNull(selector, nameof(selector));
            Dictionary<string, int> columns = ObjectBuilder.BuilderColumns(dataTable);
            ObjectBuilder buider = new ObjectBuilder(columns);
            Expression expression = this.ExpressionBody(buider, selector.Body);
            return Expression.Lambda<Func<IDataResult, T>>(expression, ADataResult.RowParameter).Compile();
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="resultDelegate">选择器</param>
        /// <returns></returns>
        public List<T> ToList<T>(DataTable dataTable, Func<IDataResult, T> resultDelegate)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            ExceptionUtils.CheckNotNull(resultDelegate, nameof(resultDelegate));
            List<T> items = new List<T>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRowResult result = new DataRowResult(dataTable.Rows[i]);
                items.Add(resultDelegate(result));
            }
            return items;
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public List<T> ToList<T>(DataTable dataTable, Expression<Func<object, T>> selector)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            ExceptionUtils.CheckNotNull(selector, nameof(selector));
            Func<IDataResult, T> resultDelegate = this.CreateInstanceDelegate(dataTable, selector);
            return this.ToList<T>(dataTable, resultDelegate);
        }
    }
}
