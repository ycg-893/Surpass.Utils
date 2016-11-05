using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;


namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元属性
    /// </summary>
    public class MetaProperty : MetaMember
    {
        /// <summary>
        /// 实例化 MetaProperty 类新实例
        /// </summary>
        /// <param name="propertyInfo">属性</param>
        public MetaProperty(PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            if (propertyInfo.CanRead)
            {
                this._GetMemberValue = MethodFactory.CreatePropertyGet<object>(propertyInfo);
            }
            else
            {
                this._GetMemberValue = null;
            }
            if (propertyInfo.CanWrite)
            {
                this._SetMemberValue = MethodFactory.CreatePropertySet<object>(propertyInfo);
            }
            else
            {
                this._SetMemberValue = null;
            }
        }

        /// <summary>
        /// 获取是否允许读
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return this.PropertyInfo.CanRead;
            }
        }

        /// <summary>
        /// 获取是否允许写
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return this.PropertyInfo.CanRead;
            }
        }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }
                

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private Func<object, object> _GetMemberValue;

        /// <summary>
        /// 获取成员值
        /// </summary>
        public sealed override Func<object, object> GetMemberValue
        {
            get
            {
                return this._GetMemberValue;
            }
        }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private Action<object, object> _SetMemberValue;

        /// <summary>
        /// 设置成员值
        /// </summary>
        public sealed override Action<object, object> SetMemberValue
        {
            get
            {
                return this._SetMemberValue;
            }
        }

        /// <summary>
        /// 获取成员
        /// </summary>
#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new MemberInfo MemberInfo
        {
            get { return this.PropertyInfo; }
        }

        /// <summary>
        /// 获取是否是属性
        /// </summary>
        public sealed override bool IsProperty
        {
            get { return true; }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this._SetMemberValue = null;
            this.PropertyInfo = null;
            this._SetMemberValue = null;

        }
    }
}
