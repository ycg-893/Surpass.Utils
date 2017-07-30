using Surpass.Utils.Reflection;
using Surpass.Utils.Reflection.Meta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Validations
{
    /// <summary>
    /// 验证管理
    /// 实现 ValidationAttribute 特性和 IValidatableObject 接口的验证
    /// </summary>
    public static class ValidationManager
    {
        /// <summary>
        /// 验证对象
        /// </summary>
        /// <param name="instance">实例</param>
        public static void Validate(object instance)
        {
            if (instance == null)
            {
                return;
            }
            Type type = instance.GetType();
            var metaPropertys = ReflectionUtils.GetMetaPropertys(type);
            Validate(metaPropertys, instance);
        }

        /// <summary>
        /// 验证对象
        /// </summary>
        /// <param name="metaPropertys">元属性集合</param>
        /// <param name="instance">实例</param>
        public static void Validate(ISet<MetaProperty> metaPropertys, object instance)
        {
            if (metaPropertys == null || instance == null)
            {
                return;
            }
            foreach (MetaProperty p in metaPropertys)
            {
                if (p.MemberInfo.CanRead)
                {
                    object value = p.GetValue(instance);
                    var context = new ValidationContext(instance, null, null)
                    {
                        DisplayName = p.DisplayAttribute != null ? p.DisplayAttribute.Name : p.MemberInfo.Name,
                        MemberName = p.MemberInfo.Name
                    };
                    foreach (var va in p.ValidationAttributes)
                    {
                        va.Validate(value, context);
                    }
                }
            }
            if (instance is IValidatableObject)
            {
                IValidatableObject validatable = (IValidatableObject)instance;
                var validationResult = validatable.Validate(new ValidationContext(instance, null, null)).FirstOrDefault();
                if (validationResult != null)
                {
                    throw new ValidationException(validationResult.ErrorMessage);
                }
            }
        }
    }
}
