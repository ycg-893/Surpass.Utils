using Surpass.Utils.DataResult;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 数据扩展
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// 输出固定Sql到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <param name="key">键，推荐使用Sql或SELECT列表</param>
        /// <returns></returns>
        public static List<T> ToFixedSqlList<T>(this IDataReader dataReader, string key)
        {
            return AObjectDataResult.ToFixedSqlList<T>(key, dataReader);
        }

        /// <summary>
        /// 输出固定Sql到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="key">键，推荐使用Sql或SELECT列表</param>
        /// <returns></returns>
        public static List<T> ToFixedSqlList<T>(this DataTable dataTable, string key)
        {
            return AObjectDataResult.ToFixedSqlList<T>(key, dataTable);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader dataReader)
        {
            ObjectDataResult<T> result = new ObjectDataResult<T>();
            return result.ToList(dataReader);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataReader">阅读器</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<object> ToList(this IDataReader dataReader, Type type)
        {
            ObjectDataResult<object> result = new ObjectDataResult<object>(type);
            return result.ToList(dataReader);
        }

        /// <summary>
        /// 返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public static T Single<T>(this IDataReader dataReader)
        {
            return DataExtensions.ToList<T>(dataReader).Single();
        }

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this IDataReader dataReader)
        {
            return DataExtensions.ToList<T>(dataReader).SingleOrDefault();
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IDataReader dataReader, Expression<Func<object, T>> selector)
        {
            AnonymousDataResult result = new AnonymousDataResult();
            return result.ToList(dataReader, selector);
        }

        /// <summary>
        /// 返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static T Single<T>(this IDataReader dataReader, Expression<Func<object, T>> selector)
        {
            return DataExtensions.ToList<T>(dataReader, selector).Single();
        }

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataReader">阅读器</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this IDataReader dataReader, Expression<Func<object, T>> selector)
        {
            return DataExtensions.ToList<T>(dataReader, selector).SingleOrDefault();
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable)
        {
            ObjectDataResult<T> result = new ObjectDataResult<T>();
            return result.ToList(dataTable);
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<object> ToList(this DataTable dataTable, Type type)
        {
            ObjectDataResult<object> result = new ObjectDataResult<object>(type);
            return result.ToList(dataTable);
        }

        /// <summary>
        /// 返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static T Single<T>(this DataTable dataTable)
        {
            return DataExtensions.ToList<T>(dataTable).Single();
        }

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this DataTable dataTable)
        {
            return DataExtensions.ToList<T>(dataTable).SingleOrDefault();
        }

        /// <summary>
        /// 输出到列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dataTable, Expression<Func<object, T>> selector)
        {
            AnonymousDataResult result = new AnonymousDataResult();
            return result.ToList(dataTable, selector);
        }

        /// <summary>
        /// 返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static T Single<T>(this DataTable dataTable, Expression<Func<object, T>> selector)
        {
            return DataExtensions.ToList<T>(dataTable, selector).Single();
        }

        /// <summary>
        /// 返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dataTable">数据表</param>
        /// <param name="selector">选择器</param>
        /// <returns></returns>
        public static T SingleOrDefault<T>(this DataTable dataTable, Expression<Func<object, T>> selector)
        {
            return DataExtensions.ToList<T>(dataTable, selector).SingleOrDefault();
        }
    }
}
