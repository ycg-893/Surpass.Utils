using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using Surpass.Utils.Properties;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 对象返回
    /// </summary>
    public abstract class AObjectDataResult
    {
        private static readonly Dictionary<Type, Dictionary<string, object>> FixedSqlDataReaderDictionary = new Dictionary<Type, Dictionary<string, object>>(); //固定Sql记录集字典
        private static readonly Dictionary<Type, Dictionary<string, object>> FixedSqlDataTableDictionary = new Dictionary<Type, Dictionary<string, object>>();  //固定Sql数据表字典

        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ResultType { get; private set; }

        /// <summary>
        /// 获取是否是常规类型(基本类型、枚举类型、Object类型)
        /// </summary>
        public bool IsGeneralType { get; private set; }

        /// <summary>
        /// 实例化 AObjectDataResult 类新实例
        /// </summary>
        /// <param name="type">类型</param>
        public AObjectDataResult(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            this.ResultType = type;
            this.IsGeneralType = type.IsBaseTypeOrNullableDefinitionBaseType() || type == typeof(object);
            if (!this.IsGeneralType)
            {
                if (!type.IsClass)
                {
                    throw new ArgumentException(string.Format(Resources.NotBaseTypeNotEqualClass, type.FullName));
                }
                if (!type.IsDefaultConstructor())
                {
                    throw new ArgumentException(string.Format(Resources.TypeNotDefaultConstructor, type.FullName));
                }
            }
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <returns></returns>
        protected Func<IDataResult, TResult> CreateInstanceDelegate<TResult>()
        {
            if (this.IsGeneralType)
            {
                var value = Expression.Call(ADataResult.RowParameter, ADataResult.GetValueMethod.MakeGenericMethod(this.ResultType)
                            , Expression.Constant(ADataResult.ScalarColumnMapping));
                return Expression.Lambda<Func<IDataResult, TResult>>(value, ADataResult.RowParameter).Compile();
            }
            else
            {
                throw new ArgumentException(string.Format(Resources.NotEqualBaseType, this.ResultType.FullName));
            }
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataReader">数据流</param>
        /// <returns></returns>
        protected virtual Func<IDataResult, TResult> CreateInstanceDelegate<TResult>(IDataReader dataReader)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            if (this.IsGeneralType)
            {
                throw new ArgumentException(string.Format(Resources.NotEqualClassOrInterfaceNotSupportedDelegate, this.ResultType.FullName));
            }
            else
            {
                try
                {
                    Dictionary<string, int> columns = ObjectBuilder.BuilderColumns(dataReader);
                    ObjectBuilder buider = new ObjectBuilder(columns);
                    Expression expression = buider.ObjectExpressionBody(this.ResultType);
                    return Expression.Lambda<Func<IDataResult, TResult>>(expression, ADataResult.RowParameter).Compile();
                }
                catch
                {
                    dataReader.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        protected virtual Func<IDataResult, TResult> CreateInstanceDelegate<TResult>(DataTable dataTable)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            if (this.IsGeneralType)
            {
                throw new ArgumentException(string.Format(Resources.NotEqualClassOrInterfaceNotSupportedDelegate, this.ResultType.FullName));
            }
            else
            {
                Dictionary<string, int> columns = ObjectBuilder.BuilderColumns(dataTable);
                ObjectBuilder buider = new ObjectBuilder(columns);
                Expression expression = buider.ObjectExpressionBody(this.ResultType);
                return Expression.Lambda<Func<IDataResult, TResult>>(expression, ADataResult.RowParameter).Compile();
            }
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="fun">函数</param>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(Func<IDataResult, TResult> fun, IDataReader dataReader)
        {
            ExceptionUtils.CheckNotNull(fun, nameof(fun));
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            using (IDataReader reader = dataReader)
            {
                List<TResult> Items = new List<TResult>();
                DataReaderResult result = new DataReaderResult(reader);
                while (reader.Read())
                {
                    Items.Add(fun(result));
                }
                return Items;
            }
        }

        private static Func<IDataResult, TResult> DataReaderFixedSqlFunResult<TResult>(string sql, IDataReader dataReader)
        {
            Type type = typeof(TResult);           
            Dictionary<string, object> keyFun;
            lock (AObjectDataResult.FixedSqlDataReaderDictionary)
            {
                if (!AObjectDataResult.FixedSqlDataReaderDictionary.TryGetValue(type, out keyFun))
                {
                    keyFun = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                    AObjectDataResult.FixedSqlDataReaderDictionary[type] = keyFun;
                }
                object fun;
                if (!keyFun.TryGetValue(sql, out fun))
                {
                    ObjectDataResult<TResult> data = new ObjectDataResult<TResult>();
                    fun = data.CreateInstanceDelegate(dataReader);
                    keyFun[sql] = fun;
                }
                return (Func<IDataResult, TResult>)fun;
            }            
        }

        /// <summary>
        /// 输出具有固定映射到
        /// </summary>
        /// <param name="sql">Sql</param>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public static List<TResult> ToFixedSqlList<TResult>(string sql, IDataReader dataReader)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(sql, nameof(sql));
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            Func<IDataResult, TResult> fun = AObjectDataResult.DataReaderFixedSqlFunResult<TResult>(sql, dataReader);
            return ToList(fun, dataReader);
        }

        /// <summary>
        /// 输出列表
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="fun">函数</param>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(Func<IDataResult, TResult> fun, DataTable dataTable)
        {
            ExceptionUtils.CheckNotNull(fun, nameof(fun));
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            List<TResult> Items = new List<TResult>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                DataRowResult result = new DataRowResult(row);
                Items.Add(fun(result));
            }
            return Items;
        }

        private static Func<IDataResult, TResult> DataTableFixedSqlFunResult<TResult>(string sql, DataTable dataTable)
        {
            Type type = typeof(TResult);
            Dictionary<string, object> keyFun;
            lock (AObjectDataResult.FixedSqlDataTableDictionary)
            {
                if (!AObjectDataResult.FixedSqlDataTableDictionary.TryGetValue(type, out keyFun))
                {
                    keyFun = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                    AObjectDataResult.FixedSqlDataTableDictionary[type] = keyFun;
                }
                object fun;
                if (!keyFun.TryGetValue(sql, out fun))
                {
                    ObjectDataResult<TResult> data = new ObjectDataResult<TResult>();
                    fun = data.CreateInstanceDelegate(dataTable);
                    keyFun[sql] = fun;
                }
                return (Func<IDataResult, TResult>)fun;
            }
        }

        /// <summary>
        /// 输出具有固定映射到
        /// </summary>
        /// <param name="sql">Sql</param>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static List<TResult> ToFixedSqlList<TResult>(string sql, DataTable dataTable)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(sql, nameof(sql));
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            Func<IDataResult, TResult> fun = AObjectDataResult.DataTableFixedSqlFunResult<TResult>(sql, dataTable);
            return ToList(fun, dataTable);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        protected List<TResult> ToList<TResult>(IDataReader dataReader)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            using (IDataReader reader = dataReader)
            {
                List<TResult> Items = new List<TResult>();
                DataReaderResult result = new DataReaderResult(reader);
                if (this.IsGeneralType)
                {                   
                    Func<IDataResult, TResult> fun = this.CreateInstanceDelegate<TResult>();
                    while (reader.Read())
                    {
                        Items.Add(fun(result));
                    }
                }
                else
                {
                    Func<IDataResult, TResult> fun = this.CreateInstanceDelegate<TResult>(dataReader);
                    while (reader.Read())
                    {
                        Items.Add(fun(result));
                    }
                }
                return Items;
            }
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        protected List<TResult> ToList<TResult>(DataTable dataTable)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            List<TResult> Items = new List<TResult>();
            if (this.IsGeneralType)
            {
                Func<IDataResult, TResult> fun = this.CreateInstanceDelegate<TResult>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    DataRowResult result = new DataRowResult(row);
                    Items.Add(fun(result));
                }
            }
            else
            {
                Func<IDataResult, TResult> fun = this.CreateInstanceDelegate<TResult>(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    DataRowResult result = new DataRowResult(row);
                    Items.Add(fun(result));
                }
            }
            return Items;
        }
    }
}
