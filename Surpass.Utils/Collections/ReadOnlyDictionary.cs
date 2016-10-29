using Surpass.Utils.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surpass.Utils.Collections
{
    /// <summary>
    /// 只读的表示键和值（字典）的集合。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">字典中的值的类型。</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
    [Serializable]
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        /// <summary>
        /// 用于保存被包装的普通字典实例。
        /// </summary>
#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private IDictionary<TKey, TValue> _dictionary;

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private IDictionary dictionary;

        /// <summary>
        /// 实例化 ReadOnlyDictionary 类新实例
        /// </summary>
        /// <param name="innerDirectionary">字典接口</param>
        /// <exception cref="ArgumentNullException">innerDirectionary为null引发的异常</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> innerDirectionary)
        {
            if (innerDirectionary == null)
            {
                throw new System.ArgumentNullException("innerDirectionary");
            }
            this._dictionary = innerDirectionary;
            this.dictionary = innerDirectionary as IDictionary;
            if (this.dictionary == null)
            {
                this.dictionary = new Dictionary<TKey, TValue>(innerDirectionary);
            }
        }

        /// <summary>
        /// 实例化 ReadOnlyDictionary 类新实例
        /// </summary>
        /// <param name="innerDirectionary">字典接口</param>
        /// <param name="comparer">比较器</param>
        /// <exception cref="ArgumentNullException">innerDirectionary为null引发的异常</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> innerDirectionary, IEqualityComparer<TKey> comparer)
        {
            if (innerDirectionary == null)
            {
                throw new System.ArgumentNullException("innerDirectionary");
            }          
            if (comparer != null)
            {
                this._dictionary = new Dictionary<TKey, TValue>(innerDirectionary, comparer);
            }
            else
            {
                this._dictionary = new Dictionary<TKey, TValue>(innerDirectionary);
            }
            this.dictionary = this._dictionary as IDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private NotSupportedException ThrowNotSupportedException(string operation)
        {
            throw new NotSupportedException(string.Format(Resources.ReadOnlyNotSupportedOperation, operation));
        }

        #region IDictionary<TKey,TValue>
        /// <summary>
        /// 在字典中添加一个带有所提供的键和值的元素。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="value">要添加的元素的值。</param>
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.ThrowNotSupportedException("Add");
        }

        /// <summary>
        /// 确定字典中是否包含指定的键。
        /// </summary>
        /// <param name="key">要在字典中定位的键。</param>
        /// <returns>如果字典中包含具有指定键的元素，则为true；否则为false。</returns>
        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 获取包含字典中的键的集合。
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return this._dictionary.Keys; }
        }

        /// <summary>
        /// 从字典中移除所指定的键的值。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        /// <returns>如果成功找到并移除该元素，则为 true；否则为 false。
        /// 如果在字典中没有找到 key，此方法则返回 false。</returns>
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw this.ThrowNotSupportedException("Remove");
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="value">当此方法返回值时，如果找到该键，便会返回与指定的键相关联的值；
        /// 否则，则会返回 value 参数的类型默认值。该参数未经初始化即被传递。</param>
        /// <returns>如果字典包含具有指定键的元素，则为 true；否则为 false。</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取包含字典中的值的集合。
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return this._dictionary.Values; }
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <returns>与指定的键相关联的值。如果找不到指定的键，
        /// get操作便会引发<see cref="KeyNotFoundException"/>异常。</returns>
        public TValue this[TKey key]
        {
            get { return this._dictionary[key]; }
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// 调用set操作总是会引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <returns>与指定的键相关联的值。如果找不到指定的键，
        /// get操作便会引发<see cref="KeyNotFoundException"/>异常。
        /// 调用set操作总是会引发<see cref="NotSupportedException"/>异常。</returns>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this[key]; }
            set
            {
                throw this.ThrowNotSupportedException("Set");
            }
        }
        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>>
        /// <summary>
        /// 在字典中添加一个带有所提供的键和值的元素。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="item">要添加的元素的键和值。</param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw this.ThrowNotSupportedException("Add");
        }

        /// <summary>
        /// 从字典中移除所有项。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw this.ThrowNotSupportedException("Clear");
        }

        /// <summary>
        /// 确定集合中是否包含特定值。
        /// </summary>
        /// <param name="item">要在集合中定位的对象。</param>
        /// <returns>如果在集合中找到<see cref="P:item"/>，则为true；否则为false。</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this._dictionary.Contains(item);
        }

        /// <summary>
        /// 从特定的数组索引开始，将集合中的元素复制到一个数组中。
        /// </summary>
        /// <param name="array">作为从集合复制的元素的目标位置的一维数组。
        /// 该数组必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><see cref="P:array"/>中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this._dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        public int Count
        {
            get { return this._dictionary.Count; }
        }

        /// <summary>
        /// 获取一个值，该值指示集合是否为只读。
        /// 此实现总是返回true。
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 从集合中移除特定对象的第一个匹配项。
        /// 此实现总是引发<see cref="NotSupportedException"/>异常。
        /// </summary>
        /// <param name="item">要从集合中移除的对象。</param>
        /// <returns>如果已从集合中成功移除<see cref="P:item"/>，则为true；否则为false。
        /// 如果在原始集合中没有找到<see cref="P:item"/>，该方法也会返回 false。</returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw this.ThrowNotSupportedException("Remove");
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>
        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>可用于循环访问集合的枚举数。</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>可用于循环访问集合的枚举数。</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this._dictionary).GetEnumerator();
        }
        #endregion

        /// <summary>
        /// 获取同步根锁定对象
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this.dictionary.SyncRoot;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            throw this.ThrowNotSupportedException("Add");
        }

        void IDictionary.Clear()
        {
            throw this.ThrowNotSupportedException("Clear");
        }

        bool IDictionary.Contains(object key)
        {
            return this.dictionary.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            throw this.ThrowNotSupportedException("Remove");
        }

        ICollection IDictionary.Values
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this.dictionary[key];
            }
            set
            {
                throw this.ThrowNotSupportedException("Set");
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.dictionary.CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return this.dictionary.IsSynchronized;
            }
        }

    }

    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionsExtension
    {
        /// <summary>
        /// 返回只读字典集合
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <returns></returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// 返回只读字典集合
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary, comparer);
        }

    }
}
