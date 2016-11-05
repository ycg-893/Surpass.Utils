using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            this.ReturnType = memberInfo.GetMemberType();
            this.IsReturnBaseType = TypeExtension.IsBaseTypeOrNullableDefinitionBaseType(this.ReturnType);
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// 获取返回类型
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// 获取是否返回基本类型
        /// </summary>
        public bool IsReturnBaseType { get; private set; }

        /// <summary>
        /// 获取成员类型
        /// </summary>
        public abstract MemberType MemberType { get; }

        /// <summary>
        /// 获取引用字段值
        /// </summary>
        /// <typeparam name="TRefField">字段类型</typeparam>
        /// <param name="refField">字段</param>
        /// <param name="createRefField">创建字段</param>
        /// <returns></returns>
        protected TRefField GetRefField<TRefField>(ref TRefField refField, Func<TRefField> createRefField)
            where TRefField : class
        {
            if (refField == null)
            {
                lock (this)
                {
                    if (refField == null)
                    {
                        refField = createRefField();
                    }
                }
            }
            return refField;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<Attribute> customAttributes = null;

        /// <summary>
        /// 获取自定义特性集合
        /// </summary>
        public IList<Attribute> CustomAttributes
        {
            get
            {
                return this.GetRefField<IList<Attribute>>(ref this.customAttributes, () =>
                     {
                         List<Attribute> atts = new List<Attribute>();
                         var customs = this.MemberInfo.GetCustomAttributes(true);
                         if (customs != null)
                         {
                             foreach (var item in customs)
                             {
                                 atts.Add((Attribute)item);
                             }
                         }
                         return atts.AsReadOnly();
                     });
            }
        }

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
            this.ReturnType = null;
            this.CustomAttributes.Clear();
            this.customAttributes = null;
        }
    }

    /// <summary>
    /// 成员类型
    /// </summary>
    public enum MemberType
    {
        /// <summary>
        /// 属性
        /// </summary>
        Property,
        /// <summary>
        /// 字段
        /// </summary>
        Field,
        /// <summary>
        /// 方法
        /// </summary>
        Method,
        /// <summary>
        /// 构造函数
        /// </summary>
        Constructor
    }
}
