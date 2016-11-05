using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元读写成员
    /// </summary>
    public abstract class MetaReadWriteMember : MetaMember
    {
        /// <summary>
        /// 实例化 MetaReadWriteMember 类新实例
        /// </summary>
        /// <param name="memberInfo"></param>
        protected MetaReadWriteMember(MemberInfo memberInfo)
            : base(memberInfo)
        {
            
        }        

        /// <summary>
        /// 获取是否允许读
        /// </summary>
        public abstract bool CanRead { get; }

        /// <summary>
        /// 获取是否允许写
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// 获取成员值
        /// </summary>
        public abstract object GetValue(object instance);

        /// <summary>
        /// 获取成员值
        /// </summary>
        public TValue GetValue<TValue>(object instance)
        {
            return (TValue)this.GetValue(instance);
        }

        /// <summary>
        /// 设置成员值
        /// </summary>
        public abstract void SetValue(object instance, object value);

    }
}
