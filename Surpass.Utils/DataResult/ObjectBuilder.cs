using Surpass.Utils.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 对象生成器
    /// </summary>
    public class ObjectBuilder
    {
        /// <summary>
        /// 获取列集合
        /// </summary>
        public Dictionary<string, int> Columns { get; private set; }

        /// <summary>
        /// 获取列集合(忽略成员列名大小写)
        /// </summary>
        protected Dictionary<string, int> IgnoreCaseColumns { get; private set; }

        /// <summary>
        /// 实例化 ObjectBuilder 类新实例
        /// </summary>
        /// <param name="columnMappings">列地图</param>
        public ObjectBuilder(IEnumerable<ColumnMapping> columnMappings)
        {
            ExceptionUtils.CheckNotNull(columnMappings, nameof(columnMappings));
            this.Columns = new Dictionary<string, int>();
            this.IgnoreCaseColumns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var col in columnMappings)
            {
                this.Columns[col.ColumnName] = col.Position;
                this.IgnoreCaseColumns[col.ColumnName] = col.Position;
            }
        }

        /// <summary>
        /// 实例化 ObjectBuilder 类新实例
        /// </summary>
        /// <param name="columns">列集合</param>
        public ObjectBuilder(Dictionary<string, int> columns)
        {
            ExceptionUtils.CheckNotNull(columns, nameof(columns));
            this.Columns = columns;
            this.IgnoreCaseColumns = new Dictionary<string, int>(columns,StringComparer.InvariantCultureIgnoreCase);            
        }

        /// <summary>
        /// 生成列集合
        /// </summary>
        /// <param name="dataReader">数据阅读器</param>
        /// <returns></returns>
        public static Dictionary<string, int> BuilderColumns(IDataReader dataReader)
        {
            ExceptionUtils.CheckNotNull(dataReader, nameof(dataReader));
            Dictionary<string, int> Columns = new Dictionary<string, int>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                Columns[dataReader.GetName(i)] = i;
            }
            return Columns;
        }

        /// <summary>
        /// 生成列集合
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static Dictionary<string, int> BuilderColumns(DataTable dataTable)
        {
            ExceptionUtils.CheckNotNull(dataTable, nameof(dataTable));
            Dictionary<string, int> Columns = new Dictionary<string, int>();
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                Columns[dataTable.Columns[i].ColumnName] = i;
            }
            return Columns;
        }

        /// <summary>
        /// 获取索引(-1表示未找到)
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public virtual int GetIndex(string memberName)
        {
            int index;
            if (this.Columns.TryGetValue(memberName, out index))
            {
                return index;
            }
            else
            {
                if (this.IgnoreCaseColumns.TryGetValue(memberName, out index))
                {
                    return index;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 获取单个表达式
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public Expression ScalarExpressionBody(Type type, string name)
        {
            int index = this.GetIndex(name);
            if (index != -1)
            {
                return Expression.Call(ADataResult.RowParameter,
                    ADataResult.GetValueMethod.MakeGenericMethod(type),
                    Expression.Constant(new ColumnMapping(index, name)));
            }
            else
            {
                object value;
                if (type.IsValueType)
                {
                    value = Activator.CreateInstance(type);
                }
                else
                {
                    if (type == typeof(string))
                    {
                        value = null;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(Resources.NotSupportedType, type.FullName));
                    }
                }
                return Expression.Constant(value, type);
            }
        }

        /// <summary>
        /// 对象成员
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="member"></param>
        /// <param name="RepeatTypes"></param>
        /// <returns></returns>
        private MemberAssignment ObjectMemberAssignment(Type memberType, MemberInfo member, HashSet<Type> RepeatTypes)
        {
            if (memberType.IsBaseTypeOrNullableDefinitionBaseTType()) //基本类型
            {
                var value = this.ScalarExpressionBody(memberType, member.Name);
                return Expression.Bind(member, value);
            }
            else
            {
                //必须防止限递归,即 A 类引用 B 类，B类引用C类，C类引用A类
                if (memberType.IsClass
                    && memberType.GetConstructor(Type.EmptyTypes) != null
                    && !memberType.IsEnumerableType()
                    && !RepeatTypes.Contains(memberType))
                {
                    var value = this.ObjectExpressionBody(memberType, RepeatTypes);
                    return Expression.Bind(member, value);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取对象类型表达式
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="RepeatTypes">重复的类型集合</param>
        /// <returns></returns>
        private Expression ObjectExpressionBody(Type type, HashSet<Type> RepeatTypes)
        {
            RepeatTypes.Add(type);
            List<MemberBinding> bindings = new List<MemberBinding>();
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.CanWrite))
            {
                MemberAssignment assignment = this.ObjectMemberAssignment(property.PropertyType, property, RepeatTypes);
                if (assignment != null)
                {
                    bindings.Add(assignment);
                }
            }
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                MemberAssignment assignment = this.ObjectMemberAssignment(field.FieldType, field, RepeatTypes);
                if (assignment != null)
                {
                    bindings.Add(assignment);
                }
            }
            return Expression.MemberInit(Expression.New(type.GetConstructor(Type.EmptyTypes)), bindings);
        }

        /// <summary>
        /// 获取对象类型表达式
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public Expression ObjectExpressionBody(Type type)
        {
            HashSet<Type> RepeatTypes = new HashSet<Type>();
            return this.ObjectExpressionBody(type, RepeatTypes);
        }        
    }
}
