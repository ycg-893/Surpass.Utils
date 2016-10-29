using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surpass.Utils.Collections
{
    /// <summary>
    /// 字典集合合变动委托
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="sender">对象</param>
    /// <param name="e">事件参数</param>
    public delegate void ObservableDictionaryChangedEventHandler<TKey, TValue>(object sender, ObservableDictionaryChangedEventArgs<TKey, TValue> e);

    /// <summary>
    /// 字典集合
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    [Serializable]
    public class ObservableDictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        /// <summary>
        /// 实例化 ObservableDictionaryChangedEventArgs 类新实例
        /// </summary>
        /// <param name="action">变动方式</param>
        public ObservableDictionaryChangedEventArgs(ObservableDictionaryChangedAction action)
        {
            this.Action = action;
            this.NewItems = new Dictionary<TKey, TValue>();
            this.OldItems = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// 实例化 ObservableDictionaryChangedEventArgs 类新实例
        /// </summary>
        /// <param name="action">变动方式</param>
        /// <param name="changedItems">变动项目</param>
        public ObservableDictionaryChangedEventArgs(ObservableDictionaryChangedAction action,
            IDictionary<TKey, TValue> changedItems)
        {
            this.Action = action;
            if (action == ObservableDictionaryChangedAction.Add)
            {
                this.NewItems = changedItems;
                this.OldItems = new Dictionary<TKey, TValue>();
            }
            else
            {
                this.NewItems = new Dictionary<TKey, TValue>();
                this.OldItems = changedItems;
            }
        }

        /// <summary>
        /// 实例化 ObservableDictionaryChangedEventArgs 类新实例
        /// </summary>
        /// <param name="action">变动方式</param>
        /// <param name="newItems">新项目列表</param>
        /// <param name="OldItems">旧项目列表</param>
        public ObservableDictionaryChangedEventArgs(ObservableDictionaryChangedAction action,
            IDictionary<TKey, TValue> newItems, IDictionary<TKey, TValue> OldItems)
        {
            this.Action = action;
            this.NewItems = newItems;
            this.OldItems = OldItems;
        }

        /// <summary>
        /// 获取动作
        /// </summary>
        public ObservableDictionaryChangedAction Action { get; private set; }        

        /// <summary>
        /// 获取更改中涉及的新项的项目列表。
        /// </summary>
        public IDictionary<TKey, TValue> NewItems { get; private set; }
       

        /// <summary>
        /// 获取更改中涉及的旧项的项目列表。
        /// </summary>
        public IDictionary<TKey, TValue> OldItems { get; private set; }
        
    }

    /// <summary>
    /// 字典集合变动动作
    /// </summary>
    [Serializable]
    public enum ObservableDictionaryChangedAction
    {
        /// <summary>
        /// 向集合中添加了一个或多个项。
        /// </summary>
        Add = 0,
        /// <summary>
        /// 从集合中移除了一个或多个项。
        /// </summary>
        Remove = 1
    }
}
