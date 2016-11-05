using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;


namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元字段
    /// </summary>
    public class MetaField : MetaReadWriteMember
    {
        /// <summary>
        /// 实例化 MetaField 类新实例
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        public MetaField(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            this.MemberInfo = fieldInfo;
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
                return !this.MemberInfo.IsInitOnly;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<object, object> readValue;

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="instance">实例</param>
        /// <returns></returns>
        public override object GetValue(object instance)
        {
            var read = this.GetRefField<Func<object, object>>(ref this.readValue, () =>
            {                
                return MethodFactory.CreateFieldGet<object>(this.MemberInfo);
            });
            return read(instance);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<object, object> writeValue;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public override void SetValue(object instance, object value)
        {
            if (!this.CanWrite)
            {
                throw new NotSupportedException("不支持写取操作");
            }
            var write = this.GetRefField<Action<object, object>>(ref this.writeValue, () =>
            {
                return MethodFactory.CreateFieldSet<object>(this.MemberInfo);               
            });
            write(instance, value);
        }

        /// <summary>
        /// 获取成员
        /// </summary>       
        public new FieldInfo MemberInfo { get; private set; }

        /// <summary>
        /// 获取元成员类型
        /// </summary>
        public sealed override MemberType MemberType
        {
            get
            {
                return MemberType.Field;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.readValue = null;
            this.MemberInfo = null;
            this.writeValue = null;
        }        
    }
}
