using Surpass.Utils.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Reflection.Meta
{
    /// <summary>
    /// 元对象，只含公用实例对象，方法不包含属性方法
    /// </summary>
    public sealed class MetaObject
    {
        /// <summary>
        /// 元缓存对象集合
        /// </summary>
        private static ConcurrentDictionary<Type, MetaObject> metaCacheObjects;

        /// <summary>
        /// 静态实例化
        /// </summary>
        static MetaObject()
        {
            metaCacheObjects = new ConcurrentDictionary<Type, MetaObject>();
        }

        /// <summary>
        /// 获取元对象，如果缓存中存在则会先取缓存，否则实例化新实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static MetaObject GetMetaObject(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            MetaObject metaObject;
            if (metaCacheObjects.TryGetValue(type, out metaObject))
            {
                return metaObject;
            }
            return new MetaObject(type);
        }

        /// <summary>
        /// 获取或添加缓存元对象，如果缓存中存在则会先取缓存，否则实例化新实例并缓存
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static MetaObject GetOrAddCacheMetaObject(Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            Lazy<MetaObject> lazy = null;
            return metaCacheObjects.GetOrAdd(type, (t) =>
            {
                if (lazy == null)
                {
                    lazy = new Lazy<MetaObject>(() =>
                    {
                        return new MetaObject(t);
                    });
                }
                return lazy.Value;
            });
        }

        /// <summary>
        /// 实例化 MetaObject 新实例
        /// </summary>
        /// <param name="type">类型</param>
        private MetaObject(Type type)
        {
            this.Type = type;
            this.Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //顺序不能改，因方法调用了属性判断
            //初始化特性集合
            this.InitAttributes();
            List<MetaMember> items = new List<MetaMember>();

            //初始构造函数
            this.InitConstructor(items);
            //初始字段
            this.InitField(items);
            //初始化属性
            this.InitProperty(items);
            //初始化方法
            this.InitMethod(items);
            this.Members = items.AsReadOnly();
        }

        /// <summary>
        /// 初始化特性集合
        /// </summary>
        private void InitAttributes()
        {
            List<Attribute> atts = new List<Attribute>();
            var customs = this.Type.GetCustomAttributes(true);
            if (customs != null)
            {
                foreach (var item in customs)
                {
                    atts.Add((Attribute)item);
                }
            }
            this.CustomAttributes = atts.AsReadOnly();
        }

        /// <summary>
        /// 
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IDictionary<TypeKey, MetaConstructor> constructorDictionary;

        /// <summary>
        /// 默认构造
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MetaConstructor defaultConstructor = null;

        /// <summary>
        /// 初始构造函数
        /// </summary>
        /// <param name="items"></param>
        private void InitConstructor(List<MetaMember> items)
        {
            Dictionary<TypeKey, MetaConstructor> constructorDic = new Dictionary<TypeKey, MetaConstructor>();
            var constructors = this.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                var metaConstructor = new MetaConstructor(constructor);
                if (metaConstructor.ConstructorTypeKey.Types.Length == 0)
                {
                    this.defaultConstructor = metaConstructor;
                }
                items.Add(metaConstructor);
                constructorDic[metaConstructor.ConstructorTypeKey] = metaConstructor;
            }
            this.constructorDictionary = new ReadOnlyDictionary<TypeKey, MetaConstructor>(constructorDic);
            this.Constructors = this.constructorDictionary.Values;
        }

        /// <summary>
        /// 初始字段
        /// </summary>
        /// <param name="items"></param>
        private void InitField(List<MetaMember> items)
        {
            Dictionary<string, MetaField> fieldDic = new Dictionary<string, MetaField>();
            var fields = this.Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var filed in fields)
            {
                var metaField = new MetaField(filed);
                items.Add(metaField);
                fieldDic[filed.Name] = metaField;
            }
            this.Fields = new ReadOnlyDictionary<string, MetaField>(fieldDic);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="items"></param>
        private void InitProperty(List<MetaMember> items)
        {
            Dictionary<string, MetaProperty> propertieDic = new Dictionary<string, MetaProperty>();
            //属性
            var properties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertie in properties)
            {
                var metaProperty = new MetaProperty(propertie);
                items.Add(metaProperty);
                propertieDic[propertie.Name] = metaProperty;
            }
            this.Propertys = new ReadOnlyDictionary<string, MetaProperty>(propertieDic);
        }

        /// <summary>
        /// 获取是否是系统方法
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns></returns>
        private bool IsSystemMethod(MethodInfo methodInfo)
        {
            if (methodInfo.Name.Equals("Equals"))
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.Equals(typeof(object)))
                {
                    return true;
                }
            }
            if ((methodInfo.Name.Equals("ToString")
                || methodInfo.Name.Equals("GetHashCode")
                || methodInfo.Name.Equals("GetType"))
                && methodInfo.GetParameters().Length == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <param name="items"></param>
        private void InitMethod(List<MetaMember> items)
        {
            Dictionary<string, List<MetaMethod>> methodDic = new Dictionary<string, List<MetaMethod>>();
            var methods = this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
            foreach (var method in methods)
            {
                bool isAdd = true;
                if (method.Name.Length > 4 && method.Name.StartsWith("get_")
                    && Propertys.ContainsKey(method.Name.Substring(4)))
                {
                    isAdd = false;
                }
                if (isAdd && method.Name.Length > 4 && method.Name.StartsWith("set_")
                    && Propertys.ContainsKey(method.Name.Substring(4)))
                {
                    isAdd = false;
                }
                if (isAdd && !IsSystemMethod(method))
                {
                    var metaMethod = new MetaMethod(method);
                    items.Add(metaMethod);
                    List<MetaMethod> lstMethod;
                    if (methodDic.TryGetValue(method.Name, out lstMethod))
                    {
                        lstMethod.Add(metaMethod);
                    }
                    else
                    {
                        lstMethod = new List<MetaMethod>();
                        lstMethod.Add(metaMethod);
                        methodDic[method.Name] = lstMethod;
                    }
                }
            }
            Dictionary<string, IList<MetaMethod>> dic = new Dictionary<string, IList<MetaMethod>>();
            foreach (var m in methodDic)
            {
                dic[m.Key] = m.Value.AsReadOnly();
            }
            this.Methods = new ReadOnlyDictionary<string, IList<MetaMethod>>(dic);
            methodDic.Clear();
        }

        /// <summary>
        /// 获取类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取构造元
        /// </summary>
        /// <param name="types">类型集</param>
        /// <returns></returns>
        public MetaConstructor GetMetaConstructor(params Type[] types)
        {
            if (types == null)
            {
                types = Type.EmptyTypes;
            }
            MetaConstructor con;
            if (constructorDictionary.TryGetValue(new TypeKey(types), out con))
            {
                return con;
            }
            return null;
        }

        /// <summary>
        /// 创建默认实例
        /// </summary>
        /// <returns></returns>
        public object CreateInstance()
        {
            if (this.defaultConstructor == null)
            {
                throw new MissingMethodException("无默认的构造函数");
            }
            return this.defaultConstructor.CreateInstance(Type.EmptyTypes);
        }

        /// <summary>
        /// 创建默认实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public T CreateInstance<T>()
        {           
            return (T)this.CreateInstance();
        }

        /// <summary>
        /// 获取自定义特性集合
        /// </summary>
        public ICollection<Attribute> CustomAttributes { get; private set; }

        /// <summary>
        /// 获取成员集合
        /// </summary>
        public ICollection<MetaMember> Members { get; private set; }

        /// <summary>
        /// 获取构造集合
        /// </summary>
        public ICollection<MetaConstructor> Constructors { get; private set; }

        /// <summary>
        /// 获取字段字典集合
        /// </summary>
        public IDictionary<string, MetaField> Fields { get; private set; }

        /// <summary>
        /// 获取属性字典集合
        /// </summary>
        public IDictionary<string, MetaProperty> Propertys { get; private set; }

        /// <summary>
        /// 获取方法字典集合(不包含属性方法)
        /// </summary>
        public IDictionary<string, IList<MetaMethod>> Methods { get; private set; }
    }
}
