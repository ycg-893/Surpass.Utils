using Surpass.Utils.Reflection.Meta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surpass.Utils.Reflection
{
    /// <summary>
    /// 反射帮助
    /// </summary>
    public static class ReflectionUtils
    {
        private static Dictionary<Type, ISet<MetaProperty>> metaPropertys = new Dictionary<Type, ISet<MetaProperty>>();

        private static Dictionary<Type, ISet<MetaField>> metaFields = new Dictionary<Type, ISet<MetaField>>();

        private static Dictionary<Type, ISet<MetaMember>> metaMembers = new Dictionary<Type, ISet<MetaMember>>();



        /// <summary>
        /// 获取成员集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static ISet<MetaMember> GetMetaMembers(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            ISet<MetaMember> members;
            lock (metaMembers)
            {
                if (metaMembers.TryGetValue(type, out members))
                {
                    return members;
                }
                members = new HashSet<MetaMember>();
                var propertys = GetMetaPropertys(type);
                var fileds = GetMetaFields(type);
                foreach(var p in propertys)
                {
                    members.Add(p);
                }
                foreach (var f in fileds)
                {
                    members.Add(f);
                }
                metaMembers[type] = members;
                return members;
            }
        }

        /// <summary>
        /// 获取属性集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static ISet<MetaProperty> GetMetaPropertys(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            ISet<MetaProperty> propertys;
            lock (metaPropertys)
            {
                if (metaPropertys.TryGetValue(type, out propertys))
                {
                    return propertys;
                }
                propertys = new HashSet<MetaProperty>();
                var ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in ps)
                {
                    propertys.Add(new MetaProperty(p));
                }
                metaPropertys[type] = propertys;
                return propertys;
            }           
        }

        /// <summary>
        /// 获取字段集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static ISet<MetaField> GetMetaFields(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            ISet<MetaField> fileds;
            lock (metaFields)
            {
                if (metaFields.TryGetValue(type, out fileds))
                {
                    return fileds;
                }
                fileds = new HashSet<MetaField>();
                var ps = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in ps)
                {
                    fileds.Add(new MetaField(p));
                }                
                metaFields[type] = fileds;
                return fileds;
            }
        }
    }
}
