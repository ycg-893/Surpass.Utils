using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 是否是整数DbType类型(Int32、UInt32、Int64、UInt64、Int16、UInt16、Byte、SByte)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIntegerDbType(this DbType type)
        {
            return type == DbType.Int32
                || type == DbType.UInt32
                || type == DbType.Int64
                || type == DbType.UInt64
                || type == DbType.Int16
                || type == DbType.UInt16
                || type == DbType.Byte
                || type == DbType.SByte;
        }

        /// <summary>
        /// 是否拥有默认的构造函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsDefaultConstructor(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// 是否属于Nullable分配类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type.IsValueType && type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// 是否属于Nullable分配类型,并输出值类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="valueType">输出值类型</param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type, out Type valueType)
        {
            if (IsNullableType(type))
            {
                valueType = type.GetGenericArguments()[0];
                return true;
            }
            valueType = null;
            return false;
        }

        /// <summary>
        /// 获取是否是枚举类型或Nullable定义的枚举
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsEnumOrNullableType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            if (type.BaseType != null && type.BaseType == typeof(Enum))
            {
                return true;
            }
            else
            {
                if (type.IsValueType && type.IsGenericType)
                {
                    return typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                        && type.GetGenericArguments()[0].BaseType == typeof(Enum);
                }
                return false;
            }
        }

        /// <summary>
        /// 是否为匿名类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type.Name.Contains("<>f__AnonymousType");
        }

        /// <summary>
        /// 是否是泛型分配类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="genericType">泛型类型</param>
        /// <returns></returns>
        public static bool IsGenericAssignableFromType(this Type type, Type genericType)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            ExceptionUtils.CheckNotNull(genericType, nameof(genericType));
            if (type.IsGenericType && genericType.IsGenericType)
            {
                Type[] argument = type.GetGenericArguments();
                Type[] genericArgument = genericType.GetGenericArguments();
                if (argument.Length == genericArgument.Length)
                {
                    try
                    {
                        //可能会导致类型约束异常
                        return genericType.MakeGenericType(argument).IsAssignableFrom(type);
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否属于泛型枚举器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGenericEnumerableType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(IEnumerable<>));
        }

        /// <summary>
        /// 是否属于枚举器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsEnumerableType(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 获取是否是字节数组
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsByteArrayType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type == typeof(byte[]);
        }

        /// <summary>
        /// 是否属于字典类型(实现 IDictionary 非泛型接口)
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsDictionaryType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type.IsGenericType && (typeof(IDictionary).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        /// <summary>
        /// 是否属于泛型字典类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGenericDictionaryType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(IDictionary<,>));
        }

        /// <summary>
        /// 是否属于 IList 分配的泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsListGenericType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(IList<>));
        }

        /// <summary>
        /// 是否属于 ISet 分配的泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSetGenericType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(ISet<>));
        }

        /// <summary>
        /// 是否属于查询类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsQueryableType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return typeof(IQueryable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否属于查询泛型类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGenericQueryableType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(IQueryable<>));
        }

        /// <summary>
        /// 是否属于分组查询类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGroupingType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(IGrouping<,>));
        }

        /// <summary>
        /// 获取一个类型是否属于数组类型或泛型集合类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsArrayOrGenericCollectionType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return type.IsArray || type.IsGenericCollectionType();
        }

        /// <summary>
        /// 获取一个类型是否属于泛型集合类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGenericCollectionType(this Type type)
        {
            return IsGenericAssignableFromType(type, typeof(ICollection<>));
        }

        /// <summary>
        /// 是否属于目标实例类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="targetInterfaceType">目标接口类型</param>
        /// <returns></returns>
        public static bool IsTargetInstanceType(this Type type, Type targetInterfaceType)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            ExceptionUtils.CheckNotNull(targetInterfaceType, nameof(targetInterfaceType));
            if (type.IsInterface)
            {
                return false;
            }
            return targetInterfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 数字类型集
        /// </summary>
        public  static readonly HashSet<Type> NumberTypeSet = new HashSet<Type>() {
            typeof(byte),typeof(sbyte),typeof(short),
            typeof(ushort),typeof(int),typeof(uint),
            typeof(long),typeof(ulong),typeof(float),
            typeof(decimal),typeof(double)
        };

        /// <summary>
        /// 基本类型集
        /// </summary>
        public static readonly HashSet<Type> BaseTypeSet = new HashSet<Type>(NumberTypeSet) {
            typeof(string),typeof(bool),typeof(char),
            typeof(Guid),typeof(DateTime),typeof(TimeSpan),
            typeof(DateTimeOffset)
        };



        private static bool IsSetTypeOrNullableDefinitionSetType(Type type, ISet<Type> comparerSet)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            if (type.IsValueType)
            {
                if (comparerSet.Contains(type))
                {
                    return true;
                }
                Type valueType;
                if (IsNullableType(type, out valueType))
                {
                    return comparerSet.Contains(valueType);
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取是否是数字类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNumberType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return NumberTypeSet.Contains(type);
        }

        /// <summary>
        /// 获取是否是数字类型或Nullable定义的数字类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsNumberTypeOrNullableDefinitionNumberType(this Type type)
        {
            return IsSetTypeOrNullableDefinitionSetType(type, NumberTypeSet);
        }

        /// <summary>
        /// 获取是否是基本类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsBaseType(this Type type)
        {
            ExceptionUtils.CheckNotNull(type, nameof(type));
            return BaseTypeSet.Contains(type);
        }

        /// <summary>
        /// 获取是否是基本型或Nullable定义的基本类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsBaseTypeOrNullableDefinitionBaseTType(this Type type)
        {
            return IsSetTypeOrNullableDefinitionSetType(type, BaseTypeSet);
        }
    }
}
