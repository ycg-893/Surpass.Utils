using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;


namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元字段
    /// </summary>
    public class MetaField : MetaMember
    {
        /// <summary>
        /// 实例化 MetaField 类新实例
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        public MetaField(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            this.FieldInfo = fieldInfo;
            this._GetMemberValue = MethodFactory.CreateFieldGet<object>(fieldInfo);
            if (fieldInfo.IsInitOnly)
            {
                this._SetMemberValue = null;
            }
            else
            {
                this._SetMemberValue = MethodFactory.CreateFieldSet<object>(fieldInfo);
            }

        }

        /// <summary>
        /// 获取是否允许读
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取是否允许写
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return !this.FieldInfo.IsInitOnly;
            }
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        public FieldInfo FieldInfo { get; private set; }

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private Func<object, object> _GetMemberValue;

        /// <summary>
        /// 获取成员值
        /// </summary>
        public override Func<object, object> GetMemberValue
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
            get { return this.FieldInfo; }
        }

        /// <summary>
        /// 获取是否是属性
        /// </summary>
        public sealed override bool IsProperty
        {
            get { return false; }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this._SetMemberValue = null;
            this.FieldInfo = null;
            this._SetMemberValue = null;
        }
    }
}
