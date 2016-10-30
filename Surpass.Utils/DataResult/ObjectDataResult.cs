using System;
using System.Collections.Generic;
using System.Data;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 对象返回数据
    /// </summary>
    public class ObjectDataResult : AObjectDataResult
    {
        /// <summary>
        /// 实例化 ObjectDataResult 类新实例
        /// </summary>
        /// <param name="type">类型</param>
        public ObjectDataResult(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <returns></returns>
        public Func<IDataResult, object> CreateInstanceDelegate()
        {
            return this.CreateInstanceDelegate<object>();
        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataReader">数据流</param>
        /// <returns></returns>
        public Func<IDataResult, object> CreateInstanceDelegate(IDataReader dataReader)
        {
            return this.CreateInstanceDelegate<object>(dataReader);

        }

        /// <summary>
        /// 创建实例委托
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public Func<IDataResult, object> CreateInstanceDelegate(DataTable dataTable)
        {
            return this.CreateInstanceDelegate<object>(dataTable);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public List<object> ToList(IDataReader dataReader)
        {
            return this.ToList<object>(dataReader);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public List<object> ToList(DataTable dataTable)
        {
            return this.ToList<object>(dataTable);

        }

    }
}
