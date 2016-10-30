using Surpass.Utils.Properties;
using System;
using System.Collections.Generic;
using System.Data;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 对象返回数据
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class ObjectDataResult<T> : AObjectDataResult
    {
        /// <summary>
        /// 实例化 ObjectDataResult 类新实例
        /// </summary>
        public ObjectDataResult()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// 实例化 ObjectDataResult 类新实例
        /// </summary>
        /// <param name="implementType">实现类型</param>
        public ObjectDataResult(Type implementType)
            : base(implementType)
        {
            ExceptionUtils.CheckNotNull(implementType, nameof(implementType));
            if (!typeof(T).IsAssignableFrom(implementType))
            {
                throw new ArgumentException(string.Format(Resources.TypeNotTypeAssignableFrom, implementType.FullName, typeof(T).FullName));
            }
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <returns></returns>
        public Func<IDataResult, T> CreateInstanceDelegate()
        {
            return this.CreateInstanceDelegate<T>();
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataReader">数据流</param>
        /// <returns></returns>
        public Func<IDataResult, T> CreateInstanceDelegate(IDataReader dataReader)
        {
            return this.CreateInstanceDelegate<T>(dataReader);
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public Func<IDataResult, T> CreateInstanceDelegate(DataTable dataTable)
        {
            return this.CreateInstanceDelegate<T>(dataTable);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public List<T> ToList(IDataReader dataReader)
        {
            return this.ToList<T>(dataReader);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public List<T> ToList(DataTable dataTable)
        {
            return this.ToList<T>(dataTable);

        }
    }
}
