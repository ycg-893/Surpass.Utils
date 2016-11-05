using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 数据返回抽象
    /// </summary>
    public abstract class ADataResult : IDataResult
    {
        /// <summary>
        /// 获取行参数
        /// </summary>
        public static readonly ParameterExpression RowParameter = Expression.Parameter(typeof(IDataResult), "row");

        /// <summary>
        /// 获取值方法
        /// </summary>
        public static readonly MethodInfo GetValueMethod = typeof(IDataResult).GetMethod("GetValue", new Type[] { typeof(ColumnMapping) });

        /// <summary>
        /// 获取单列地图
        /// </summary>
        public readonly static ColumnMapping ScalarColumnMapping = new ColumnMapping(0, "");

        /// <summary>
        /// 实例化 ADataResult 类新实例
        /// </summary>
        protected ADataResult()
        {
        }

        /// <summary>
        /// 获取泛型值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="columnMapping">列地图</param>
        /// <returns></returns>
        public T GetValue<T>(ColumnMapping columnMapping)
        {
            object value = this.GetValue(null, columnMapping);
            if (!(value is T)) //类型相同则不转换
            {
                value = DataConvertUtils.TargetValueConvert(typeof(T), value);
            }
            else
            {
                if (value == DBNull.Value)
                {
                    value = null;
                }
            }
            return (T)value;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="columnMapping">列地图</param>
        /// <returns></returns>
        public abstract object GetValue(Type type, ColumnMapping columnMapping);

    }
}
