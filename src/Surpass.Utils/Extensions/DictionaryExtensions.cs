﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 字典扩展
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 获取或返回默认值
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键</param>
        /// <returns>指定键不存在则返回该值类型默认值</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            ExceptionUtils.CheckNotNull(dictionary, nameof(dictionary));
             TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        /// <summary>
        /// 获取或添加
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键</param>
        /// <param name="factory">生成值的工厂</param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {           
            ExceptionUtils.CheckNotNull(dictionary, nameof(dictionary));
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// 判断并移除
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键</param>
        /// <param name="value">输出值</param>
        /// <returns></returns>
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            ExceptionUtils.CheckNotNull(dictionary, nameof(dictionary));           
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }
            return false;
        }
    }
}
