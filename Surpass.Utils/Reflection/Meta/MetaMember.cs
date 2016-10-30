using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元成员
    /// </summary>
    public abstract class MetaMember : IDisposable
    {
        /// <summary>
        /// 实例化 MetaMember 类新实例
        /// </summary>
        /// <param name="memberInfo">成员</param>
        protected MetaMember(MemberInfo memberInfo)
        {
            this.MemberInfo = ExceptionUtils.CheckNotNull(memberInfo, nameof(memberInfo));
            List<Attribute> atts = new List<Attribute>();
            List<ValidationAttribute> vatts = new List<ValidationAttribute>();
            var customs = this.MemberInfo.GetCustomAttributes(true);
            DisplayAttribute displayAttribute = null;
            if (customs != null)
            {
                foreach (var item in customs)
                {
                    atts.Add((Attribute)item);
                    if (item is ValidationAttribute)
                    {
                        vatts.Add((ValidationAttribute)item);
                    }
                    else if(item is DisplayAttribute)
                    {
                        displayAttribute = (DisplayAttribute)item;
                    }
                }
            }
            if (memberInfo is PropertyInfo)
            {
                this.MemberType = ((PropertyInfo)memberInfo).PropertyType;                
            }
            else
            {
                this.MemberType = ((FieldInfo)memberInfo).FieldType;              
            }
            this.IsBaseTypeMember = TypeExtension.IsBaseTypeOrNullableDefinitionBaseTType(this.MemberType);
            this.CustomAttributes = atts.AsReadOnly();
            this.ValidationAttributes = vatts.AsReadOnly();
            this.DisplayAttribute = displayAttribute;
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// 获取成员类型
        /// </summary>
        public Type MemberType { get; private set; }

        /// <summary>
        /// 获取是否是基本类型成员(即支持直接转换)
        /// </summary>
        public bool IsBaseTypeMember { get; private set; }

        /// <summary>
        /// 获取是否是属性
        /// </summary>
        public abstract bool IsProperty { get; }     
        
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
        public abstract Func<object, object> GetMemberValue { get; }

        /// <summary>
        /// 设置成员值
        /// </summary>
        public abstract Action<object, object> SetMemberValue { get; }

        /// <summary>
        /// 获取自定义特性集合
        /// </summary>
        public IList<Attribute> CustomAttributes { get; private set; }

        /// <summary>
        /// 获取验证特性集合
        /// </summary>
        public IList<ValidationAttribute> ValidationAttributes { get; private set; }

        /// <summary>
        /// 获取友好特性
        /// </summary>
        public DisplayAttribute DisplayAttribute { get; private set; }

        /// <summary>
        /// 输出成员名称
        /// </summary>
        /// <returns></returns>
        public sealed override string ToString()
        {
            return this.MemberInfo.Name;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            this.MemberInfo = null;
            this.MemberType = null;
            this.CustomAttributes = null;
            this.ValidationAttributes = null;
        }
    }
}
