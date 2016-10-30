using System.Collections.Generic;
using System.Diagnostics;

namespace Surpass.Utils.Collections
{
    /// <summary>
    /// 字典调试视图
    /// </summary>
    public class DictionaryDebugView<K, V>
    {
        private IDictionary<K, V> dict;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public DictionaryDebugView(IDictionary<K, V> dictionary)
        {
            if (dictionary == null)
            {
                throw new System.ArgumentNullException("dictionary");
            }
            this.dict = dictionary;
        }

        /// <summary>
        /// 获取项目集合
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K, V>[] Items
        {
            get
            {
                KeyValuePair<K, V>[] array = new KeyValuePair<K, V>[this.dict.Count];
                this.dict.CopyTo(array, 0);
                return array;
            }
        }

    }
}
