using System;
using System.Data;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 数据流返回
    /// </summary>
    public sealed class DataReaderResult : ADataResult, IDataResult
    {
        /// <summary>
        /// 获取数据阅读器
        /// </summary>
        private readonly IDataReader DataReader;

        /// <summary>
        /// 实例化 DataReaderResult 类新实例
        /// </summary>
        /// <param name="dataReader">数据阅读器</param>
        public DataReaderResult(IDataReader dataReader)
        {
            this.DataReader = dataReader;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="columnMapping">列地图</param>
        /// <returns></returns>
        public override object GetValue(Type type, ColumnMapping columnMapping)
        {
            return this.DataReader.GetValue(columnMapping.Position);
        }
    }
}
