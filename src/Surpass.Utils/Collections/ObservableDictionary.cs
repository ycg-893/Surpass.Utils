using Surpass.Utils.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Surpass.Utils.Collections
{
    /// <summary>
    /// 具有变动通知的安全字典
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
    [Serializable]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private Dictionary<TKey, TValue> dictionary;

        /// <summary>
        /// 实例化 ObservableDictionary 类新实例
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ObservableDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// 实例化 ObservableDictionary 类新实例
        /// </summary>
        /// <param name="capacity">初始容量</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ObservableDictionary(int capacity)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// 实例化 ObservableDictionary 类新实例
        /// </summary>
        /// <param name="comparer">比较器</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// 实例化 ObservableDictionary 类新实例
        /// </summary>
        /// <param name="capacity">初始容量</param>
        /// <param name="comparer">比较器</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        /// <summary>
        /// 集合发生变动时的事件
        /// </summary>
        [field: NonSerialized]
        public event ObservableDictionaryChangedEventHandler<TKey, TValue> CollectionChanged;

        /// <summary>
        /// 集合变动时
        /// </summary>
        /// <param name="e">通知事件</param>
        protected virtual void OnCollectionChanged(ObservableDictionaryChangedEventArgs<TKey, TValue> e)
        {
            lock (this.SyncRoot)
            {
                var changed = this.CollectionChanged;
                if (changed != null)
                {
                    changed(this, e);
                }
            }
        }

        /// <summary>
        /// 将指定的键和值添加到字典中。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TKey key, TValue value)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue v;
                if (this.dictionary.TryGetValue(key, out v))
                {
                    throw new NotSupportedException(string.Format(Resources.ReadOnlyNotSupportedOperation, key));
                }
                else
                {
                    this.dictionary.Add(key, value);
                    Dictionary<TKey, TValue> ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = v;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                }
            }
        }

        /// <summary>
        /// 获取,不存在则返回默认值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue value;
                if (this.dictionary.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return default(TValue);
                }
            }
        }

        /// <summary>
        /// 获取并移除,不存在则返回默认值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TValue GetAndRemove(TKey key)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue value;
                if (this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary.Remove(key);
                    Dictionary<TKey, TValue> ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = value;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));
                    return value;
                }
                else
                {
                    return default(TValue);
                }
            }
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到 ObservableDictionary 中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue oldValue;
                if (this.dictionary.TryGetValue(key, out oldValue))
                {
                    return oldValue;
                }
                else
                {
                    this.dictionary[key] = value;
                    var ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = value;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                    return value;
                }
            }
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到 ObservableDictionary 中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valueFactory">值工厂</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue oldValue;
                if (this.dictionary.TryGetValue(key, out oldValue))
                {
                    return oldValue;
                }
                else
                {
                    TValue value = valueFactory(key);
                    this.dictionary[key] = value;
                    var ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = value;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                    return value;
                }
            }
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到 ObservableDictionary 中，如果指定的键已存在，则更新 ObservableDictionary 中的键/值对。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="addValue">添加的值</param>
        /// <param name="updateValueFactory">更新工厂</param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue oldValue;
                Dictionary<TKey, TValue> ChangedItem;
                if (this.dictionary.TryGetValue(key, out oldValue))
                {
                    ExceptionUtils.CheckNotNull(updateValueFactory, nameof(updateValueFactory));
                    var value = updateValueFactory(key, oldValue);
                    if (!object.Equals(value, oldValue))
                    {
                        this.dictionary[key] = value;

                        ChangedItem = new Dictionary<TKey, TValue>();
                        ChangedItem[key] = oldValue;
                        this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));

                        ChangedItem = new Dictionary<TKey, TValue>();
                        ChangedItem[key] = value;
                        this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));

                    }
                    return value;
                }
                else
                {
                    this.dictionary[key] = addValue;
                    ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = addValue;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                    return addValue;
                }
            }
        }

        /// <summary>
        /// 如果指定的键尚不存在，则将键/值对添加到 ObservableDictionary 中，如果指定的键已存在，则更新 ObservableDictionary 中的键/值对。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="addValueFactory">添加的值工厂</param>
        /// <param name="updateValueFactory">更新工厂</param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue oldValue;
                Dictionary<TKey, TValue> ChangedItem;
                if (this.dictionary.TryGetValue(key, out oldValue))
                {
                    ExceptionUtils.CheckNotNull(updateValueFactory, nameof(updateValueFactory));
                    var value = updateValueFactory(key, oldValue);
                    if (!object.Equals(value, oldValue))
                    {
                        this.dictionary[key] = value;

                        ChangedItem = new Dictionary<TKey, TValue>();
                        ChangedItem[key] = oldValue;
                        this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));

                        ChangedItem = new Dictionary<TKey, TValue>();
                        ChangedItem[key] = value;
                        this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));

                    }
                    return value;
                }
                else
                {
                    ExceptionUtils.CheckNotNull(addValueFactory, nameof(addValueFactory));
                    var value = addValueFactory(key);
                    this.dictionary[key] = value;
                    ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = value;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                    return value;
                }
            }
        }

        /// <summary>
        /// 确定 ObservableDictionary 是否包含指定的键。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                return this.dictionary.ContainsKey(key);
            }
        }

        /// <summary>
        /// 获取包含 ObservableDictionary 中的键的集合。
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.dictionary.Keys;
                }
            }
        }

        /// <summary>
        /// 从 ObservableDictionary 中移除所指定的键的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            ExceptionUtils.CheckNotNull(key, nameof(key));
            lock (this.SyncRoot)
            {
                TValue value;
                if (this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary.Remove(key);
                    Dictionary<TKey, TValue> ChangedItem = new Dictionary<TKey, TValue>();
                    ChangedItem[key] = value;
                    this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">输出的值</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this.SyncRoot)
            {
                return this.dictionary.TryGetValue(key, out value);
            }
        }

        /// <summary>
        /// 获取包含 ObservableDictionary 中的值的集合。
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.dictionary.Values;
                }
            }
        }

        /// <summary>
        /// 获取或设置相关键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                ExceptionUtils.CheckNotNull(key, nameof(key));
                lock (this.SyncRoot)
                {
                    return this.dictionary[key];
                }
            }
            set
            {
                ExceptionUtils.CheckNotNull(key, nameof(key));
                lock (this.SyncRoot)
                {
                    Dictionary<TKey, TValue> ChangedItem;
                    TValue oldValue;
                    if (this.dictionary.TryGetValue(key, out oldValue))
                    {
                        if (!object.Equals(value, oldValue))
                        {
                            this.dictionary[key] = value;

                            ChangedItem = new Dictionary<TKey, TValue>();
                            ChangedItem[key] = oldValue;
                            this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));

                            ChangedItem = new Dictionary<TKey, TValue>();
                            ChangedItem[key] = value;
                            this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                        }
                    }
                    else
                    {
                        this.dictionary[key] = value;

                        ChangedItem = new Dictionary<TKey, TValue>();
                        ChangedItem[key] = value;
                        this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Add, ChangedItem));
                    }
                }

            }
        }

        /// <summary>
        /// 清除集合
        /// </summary>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                Dictionary<TKey, TValue> ChangedItem = new Dictionary<TKey, TValue>(this.dictionary);
                this.dictionary.Clear();
                this.OnCollectionChanged(new ObservableDictionaryChangedEventArgs<TKey, TValue>(ObservableDictionaryChangedAction.Remove, ChangedItem));
            }
        }

        /// <summary>
        /// 获取包含在 ObservableDictionary 中的键/值对的数目。
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.dictionary.Count;
                }
            }
        }

        /// <summary>
        /// 返回循环访问 ObservableDictionary 的枚举器。     
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.SyncRoot)
            {
                return this.dictionary.GetEnumerator();
            }
        }


        #region 隐藏接口的成员

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private object _syncRoot = null;

        /// <summary>
        /// 同步对象
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            lock (this.SyncRoot)
            {
                this.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (this.SyncRoot)
            {
                return (this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (this.SyncRoot)
            {
                (this.dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            lock (this.SyncRoot)
            {
                return this.dictionary.GetEnumerator();
            }
        }


        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            lock (this.SyncRoot)
            {
                IDictionary dic = this.dictionary;
                return dic.GetEnumerator();
            }
        }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        ICollection IDictionary.Keys
        {
            get
            {
                lock (this.SyncRoot)
                {
                    IDictionary dic = this.dictionary;
                    return dic.Keys;
                }
            }
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        ICollection IDictionary.Values
        {
            get
            {
                lock (this.SyncRoot)
                {
                    IDictionary dic = this.dictionary;
                    return dic.Values;
                }
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        #endregion

        /// <summary>
        /// 加锁
        /// </summary>
        public void Lock()
        {
            Monitor.Enter(this.SyncRoot);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void UnLock()
        {
            Monitor.Exit(this.SyncRoot);
        }

    }
}
