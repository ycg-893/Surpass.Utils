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
        private static ConcurrentDictionary<Type, ISet<MetaProperty>> Validations =
            new ConcurrentDictionary<Type, ISet<MetaProperty>>();

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
            Lazy<ISet<MetaProperty>> lazy = null;
            ISet<MetaProperty> metaPropertys = Validations.GetOrAdd(type, (t) =>
            {
                if (lazy == null)
                {
                    lazy = GetLazy(t);
                }
                return lazy.Value;
            });
            Validate(instance, metaPropertys);
        }

        private static Lazy<ISet<MetaProperty>> GetLazy(Type type)
        {
            return new Lazy<ISet<MetaProperty>>(() =>
            {
                return GetMetaPropertys(type);
            });
        }

        private static void Validate(object instance, ISet<MetaProperty> metaPropertys)
        {
            foreach (MetaProperty p in metaPropertys)
            {
                object value = p.GetValue(instance);
                ValidationContext context = new ValidationContext(instance)
                {
                    DisplayName = p.DisplayAttribute != null ? p.DisplayAttribute.Name : p.MemberInfo.Name,
                    MemberName = p.MemberInfo.Name
                };
                foreach (var va in p.ValidationAttributes)
                {
                    va.Validate(value, context);
                }
            }
            if (instance is IValidatableObject)
            {
                IValidatableObject validatable = (IValidatableObject)instance;
                var validationResult = validatable.Validate(new ValidationContext(instance)).FirstOrDefault();
                if (validationResult != null)
                {
                    throw new System.ComponentModel.DataAnnotations.ValidationException(validationResult.ErrorMessage);
                }
            }
        }

        private static ISet<MetaProperty> GetMetaPropertys(Type type)
        {
            ISet<MetaProperty> metaPropertys = new HashSet<MetaProperty>();
            var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in ps)
            {
                if (p.CanRead && p.IsDefined(typeof(ValidationAttribute), true))
                {
                    metaPropertys.Add(new MetaProperty(p));
                }
            }
            return metaPropertys;
        }
    }
}
