using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 成员扩展
    /// </summary>
    public static class MemberExtension
    {
        /// <summary>
        /// 获取成员类型
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo member)
        {
            ExceptionUtils.CheckNotNull(member, nameof(member));
            if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            if (member is MethodInfo)
            {
                return ((MethodInfo)member).ReturnType;
            }
            if (member is ConstructorInfo)
            {
                return typeof(void);
            }
            throw new NotSupportedException("不支持的成员类型 " + member.ToString());
        }

        /// <summary>
        /// 获取成员的 T 类型特性，未配置返回 null
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static T GetMemberInfoAttribute<T>(this MemberInfo member) where T : Attribute
        {
            ExceptionUtils.CheckNotNull(member, nameof(member));
            var Attributes = member.GetCustomAttributes(typeof(T), true);
            if (Attributes != null && Attributes.Length > 0)
            {
                return (T)Attributes[0];
            }
            return null;
        }

        /// <summary>
        /// 获取成员的 T 类型特性集合
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        public static List<T> GetMemberInfoAttributes<T>(this MemberInfo member) where T : Attribute
        {
            ExceptionUtils.CheckNotNull(member, nameof(member));
            var Attributes = member.GetCustomAttributes(typeof(T), true);
            List<T> items = new List<T>();
            if (Attributes != null && Attributes.Length > 0)
            {
                foreach (var att in Attributes)
                {
                    items.Add((T)att);
                }
            }
            return items;
        }


    }
}
