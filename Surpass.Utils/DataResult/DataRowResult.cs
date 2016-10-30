using System;
using System.Data;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 数据行返回
    /// </summary>
    public sealed class DataRowResult : ADataResult
    {
        /// <summary>
        /// 获取数据阅读器
        /// </summary>
        private readonly DataRow DataRow;

        /// <summary>
        /// 实例化 ADataResult 类新实例
        /// </summary>
        /// <param name="dataRow">数据行</param>
        public DataRowResult(DataRow dataRow)
        {
            this.DataRow = dataRow;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="columnMapping">地图</param>
        /// <returns></returns>
        public override object GetValue(Type type, ColumnMapping columnMapping)
        {
            return this.DataRow[columnMapping.Position];
        }


    }
}
