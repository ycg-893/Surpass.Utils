using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 列地图
    /// </summary>
    public class ColumnMapping
    {
        /// <summary>
        /// 获取位置
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// 获取列名称
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 实例化 ColumnMapping 新实例
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="columnName">列名称</param>
        public ColumnMapping(int position, string columnName)
        {
            this.Position = position;
            this.ColumnName = columnName;
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} : {1}", this.ColumnName, this.Position);
        }
    }
}
