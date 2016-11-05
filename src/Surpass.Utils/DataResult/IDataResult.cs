using System;

namespace Surpass.Utils.DataResult
{
    /// <summary>
    /// 数据返回接口
    /// </summary>
    public interface IDataResult
    {
        /// <summary>
        /// 获取泛型值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="columnMapping">列地图</param>
        /// <returns></returns>
        T GetValue<T>(ColumnMapping columnMapping);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="columnMapping">列地图</param>
        /// <returns></returns>
        object GetValue(Type type, ColumnMapping columnMapping);
    }
}
