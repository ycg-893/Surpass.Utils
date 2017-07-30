using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Surpass.Utils
{
    /// <summary>
    /// 类型帮助
    /// </summary>
    public static class TypeUtils
    {
        private static ConcurrentDictionary<Type, Func<object>> typeInstanceDelegates = new ConcurrentDictionary<Type, Func<object>>();

        /// <summary>
        /// 创建默认构造的委托
        /// </summary>
        /// <param name="constructorInfo">构造信息</param>
        /// <returns></returns>
        public static Func<object> CreateDelegate(ConstructorInfo constructorInfo)
        {
            ExceptionUtils.CheckNotNull(constructorInfo, nameof(constructorInfo));
            var newExp = Expression.New(constructorInfo);
            Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(newExp, null);
            return lambdaExp.Compile();
        }

        /// <summary>
        /// 创建默认构造的委托
        /// </summary>
        /// <param name="constructorInfo">构造信息</param>
        /// <returns></returns>
        public static Func<T> CreateDelegate<T>(ConstructorInfo constructorInfo)
        {
            ExceptionUtils.CheckNotNull(constructorInfo, nameof(constructorInfo));
            var newExp = Expression.New(constructorInfo);
            Expression<Func<T>> lambdaExp = Expression.Lambda<Func<T>>(newExp, null);
            return lambdaExp.Compile();
        }

        /// <summary>
        /// 获取或创建委托
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <returns></returns>
        public static Func<object> GetOrCreateDelegate(ConstructorInfo constructorInfo)
        {
            ExceptionUtils.CheckNotNull(constructorInfo, nameof(constructorInfo));
            return typeInstanceDelegates.LazyGetOrAdd(constructorInfo.DeclaringType, (t) =>
            {
                return CreateDelegate(constructorInfo);
            });
        }

        /// <summary>
        /// 获取或创建委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object> GetOrCreateDelegate(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return typeInstanceDelegates.LazyGetOrAdd(type, (t) =>
            {
                ConstructorInfo constructorInfo = t.GetConstructor(Type.EmptyTypes);
                if (constructorInfo == null)
                {
                    throw new ArgumentException(string.Format("类型 {0} 无默认的构造函数。", t.FullName), nameof(type));
                }
                return CreateDelegate(constructorInfo);
            });
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object CreateInstance(Type type)
        {
            return GetOrCreateDelegate(type)();
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// 查找默认构造类型集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<Type> FindDefaultConstructorTypes(Type type)
        {
            return FindDefaultConstructorTypes(type, type.Assembly);
        }

        /// <summary>
        /// 查找默认构造类型集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static List<Type> FindDefaultConstructorTypes(Type type, Assembly assembly)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            if (assembly == null)
            {
                assembly = type.Assembly;
            }
            var types = assembly.GetTypes();
            var items = new List<Type>();
            foreach (var t in types)
            {
                ConstructorInfo constructorInfo = null;
                if (t.IsClass && !t.IsAbstract && !t.IsGenericType
                    && type.IsAssignableFrom(t)
                    && (constructorInfo = t.GetConstructor(Type.EmptyTypes)) != null)
                {
                    items.Add(t);
                }
            }
            return items;
        }

        /// <summary>
        /// 查找默认构造类型集合
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static List<Type> FindDefaultConstructorTypes<T>(Assembly assembly)
        {
            return FindDefaultConstructorTypes(typeof(T), assembly);
        }

        /// <summary>
        /// 查找默认构造类型集合
        /// </summary>
        /// <returns></returns>
        public static List<Type> FindDefaultConstructorTypes<T>()
        {
            return FindDefaultConstructorTypes(typeof(T), typeof(T).Assembly);
        }

        /// <summary>
        /// 创建默认构造委托集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        public static List<Func<T>> CreateDefaultConstructorDelegates<T>(Assembly assembly)
        {
            var types = FindDefaultConstructorTypes<T>(assembly);
            var items = new List<Func<T>>();
            foreach (var type in types)
            {
                var newExp = Expression.New(type);
                Expression<Func<T>> lambdaExp = Expression.Lambda<Func<T>>(newExp, null);
                items.Add(lambdaExp.Compile());
            }
            return items;
        }

        /// <summary>
        /// 创建默认构造委托集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static List<Func<T>> CreateDefaultConstructorDelegates<T>()
        {
            return CreateDefaultConstructorDelegates<T>(typeof(T).Assembly);
        }

        /// <summary>
        /// 创建默认构造委托集合
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<Func<object>> CreateDefaultConstructorDelegates(Type type, Assembly assembly)
        {
            var types = FindDefaultConstructorTypes(type, null);
            var items = new List<Func<object>>();
            foreach (var t in types)
            {
                items.Add(GetOrCreateDelegate(t));
            }
            return items;
        }

        /// <summary>
        /// 创建默认构造委托集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Func<object>> CreateDefaultConstructorDelegates(Type type)
        {
            return CreateDefaultConstructorDelegates(type, null);
        }
    }
}
