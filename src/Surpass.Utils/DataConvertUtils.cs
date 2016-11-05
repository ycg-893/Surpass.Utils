using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surpass.Utils
{
    /// <summary>
    /// 数据转换帮助
    /// </summary>
    public static class DataConvertUtils
    {
        /// <summary>
        /// 获取不依赖区域信息
        /// </summary>
        public static readonly System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

#if !DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private static Dictionary<Type, Func<Type, object, object>> _TargetConverter;

        /// <summary>
        /// 静态实例
        /// </summary>
        static DataConvertUtils()
        {
            DataConvertUtils._TargetConverter = new Dictionary<Type, Func<Type, object, object>>();
            DataConvertUtils._TargetConverter[typeof(string)] = DataConvertUtils.ToStringConvert;
            DataConvertUtils._TargetConverter[typeof(byte)] = DataConvertUtils.ToByteConvert;
            DataConvertUtils._TargetConverter[typeof(byte?)] = DataConvertUtils.ToByteConvert;
            DataConvertUtils._TargetConverter[typeof(sbyte)] = DataConvertUtils.ToSByteConvert;
            DataConvertUtils._TargetConverter[typeof(sbyte?)] = DataConvertUtils.ToSByteConvert;
            DataConvertUtils._TargetConverter[typeof(short)] = DataConvertUtils.ToInt16Convert;
            DataConvertUtils._TargetConverter[typeof(short?)] = DataConvertUtils.ToInt16Convert;
            DataConvertUtils._TargetConverter[typeof(ushort)] = DataConvertUtils.ToUInt16Convert;
            DataConvertUtils._TargetConverter[typeof(ushort?)] = DataConvertUtils.ToUInt16Convert;
            DataConvertUtils._TargetConverter[typeof(int)] = DataConvertUtils.ToInt32Convert;
            DataConvertUtils._TargetConverter[typeof(int?)] = DataConvertUtils.ToInt32Convert;
            DataConvertUtils._TargetConverter[typeof(uint)] = DataConvertUtils.ToUInt32Convert;
            DataConvertUtils._TargetConverter[typeof(uint?)] = DataConvertUtils.ToUInt32Convert;
            DataConvertUtils._TargetConverter[typeof(long)] = DataConvertUtils.ToInt64Convert;
            DataConvertUtils._TargetConverter[typeof(long?)] = DataConvertUtils.ToInt64Convert;
            DataConvertUtils._TargetConverter[typeof(ulong)] = DataConvertUtils.ToUInt64Convert;
            DataConvertUtils._TargetConverter[typeof(ulong?)] = DataConvertUtils.ToUInt64Convert;
            DataConvertUtils._TargetConverter[typeof(float)] = DataConvertUtils.ToSingleConvert;
            DataConvertUtils._TargetConverter[typeof(float?)] = DataConvertUtils.ToSingleConvert;
            DataConvertUtils._TargetConverter[typeof(double)] = DataConvertUtils.ToDoubleConvert;
            DataConvertUtils._TargetConverter[typeof(double?)] = DataConvertUtils.ToDoubleConvert;
            DataConvertUtils._TargetConverter[typeof(decimal)] = DataConvertUtils.ToDecimalConvert;
            DataConvertUtils._TargetConverter[typeof(decimal?)] = DataConvertUtils.ToDecimalConvert;
            DataConvertUtils._TargetConverter[typeof(bool)] = DataConvertUtils.ToBooleanConvert;
            DataConvertUtils._TargetConverter[typeof(bool?)] = DataConvertUtils.ToBooleanConvert;
            DataConvertUtils._TargetConverter[typeof(char)] = DataConvertUtils.ToCharConvert;
            DataConvertUtils._TargetConverter[typeof(char?)] = DataConvertUtils.ToCharConvert;
            DataConvertUtils._TargetConverter[typeof(Guid)] = DataConvertUtils.ToGuidConvert;
            DataConvertUtils._TargetConverter[typeof(Guid?)] = DataConvertUtils.ToGuidConvert;
            DataConvertUtils._TargetConverter[typeof(DateTime)] = DataConvertUtils.ToDateTimeConvert;
            DataConvertUtils._TargetConverter[typeof(DateTime?)] = DataConvertUtils.ToDateTimeConvert;
            DataConvertUtils._TargetConverter[typeof(TimeSpan)] = DataConvertUtils.ToTimeSpanConvert;
            DataConvertUtils._TargetConverter[typeof(TimeSpan?)] = DataConvertUtils.ToTimeSpanConvert;
            DataConvertUtils._TargetConverter[typeof(DateTimeOffset)] = DataConvertUtils.ToDateTimeOffsetConvert;
            DataConvertUtils._TargetConverter[typeof(DateTimeOffset?)] = DataConvertUtils.ToDateTimeOffsetConvert;
        }



        #region convert

        /// <summary>
        /// 字节数组转换为字符
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="removeDashes">移除中间分隔符</param>
        /// <returns></returns>
        public static string BytesToHex(byte[] bytes, bool removeDashes)
        {
            ExceptionUtils.CheckNotNull(bytes, nameof(bytes));
            string hex = BitConverter.ToString(bytes);
            if (removeDashes)
            {
                hex = hex.Replace("-", "");
            }
            return hex;
        }

        /// <summary>
        /// 字符转换为字节数组
        /// </summary>
        /// <param name="hex">字符</param>
        /// <returns></returns>
        public static byte[] HexToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return null;
            }
            string fixedHex = hex.Replace("-", string.Empty);
            byte[] bytes = new byte[fixedHex.Length / 2];
            int shift = 4;
            int offset = 0;
            foreach (char c in fixedHex)
            {
                int b = (c - '0') % 32;
                if (b > 9)
                {
                    b -= 7;
                }
                bytes[offset] |= (byte)(b << shift);
                shift ^= 4;
                if (shift != 0)
                {
                    offset++;
                }
            }
            return bytes;
        }

        /// <summary>
        /// 获取目标转换器(可自定义添加)
        /// </summary>
        public static Dictionary<Type, Func<Type, object, object>> TargetConverter
        {
            get { return DataConvertUtils._TargetConverter; }
        }

        /// <summary>
        /// 目标值转换
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        public static object TargetValueConvert(Type targetType, object value)
        {
            if (targetType == null)
            {
                return value;
            }
            if (value == null || value == DBNull.Value)
            {
                if (targetType.IsValueType)
                {
                    return Activator.CreateInstance(targetType);
                }
                return null;
            }
            Type type = value.GetType();
            if (targetType.Equals(type))
            {
                return value;
            }
            Type genericType;
            if (targetType.IsNullableType(out genericType))
            {
                targetType = genericType;
                if (genericType.Equals(type))
                {
                    return value;
                }
            }
            Func<Type, object, object> fun;
            if (DataConvertUtils.TargetConverter.TryGetValue(targetType, out fun))
            {
                return fun(targetType, value);
            }
            else
            {
                if (value != null && targetType.IsEnumOrNullableType())
                {
                    return DataConvertUtils.ToEnumConvert(targetType, value);
                }
                return value;
            }
        }

        /// <summary>
        /// 目标值转换
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        public static T? TargetValueConvert<T>(object value) where T : struct
        {
            if (value is T)
            {
                return (T)value;
            }
            if (value == null)
            {
                return null;
            }
            Type targetType = typeof(T);
            return (T)DataConvertUtils.TargetValueConvert(targetType, value);
        }

        /// <summary>
        /// 是否允许目标转换
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static bool IsAllowTargetConvert(Type targetType)
        {
            if (targetType == null)
            {
                return false;
            }
            if (TargetConverter.ContainsKey(targetType))
            {
                return true;
            }
            else
            {
                return targetType.IsEnumOrNullableType();
            }
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="fun">函数</param>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        public static T? ToValueTypeConvert<T>(Func<object, T> fun, Type targetType, object value) where T : struct
        {
            if (value == null || value == DBNull.Value)
            {
                if (targetType == typeof(T?))
                {
                    return null;
                }
                return default(T);
            }
            Type type = value.GetType();
            if (type == targetType)  //类型相同时
            {
                return (T)value;
            }
            if (type.IsValueType)
            {
                return fun(value);
            }
            else
            {
                if (type == typeof(string))
                {
                    return fun(value.ToString().Trim());
                }
                else
                {
                    return fun(value);
                }
            }
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="fun">函数</param>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        public static T? ToValueTypeConvert<T>(Func<string, T> fun, Type targetType, object value) where T : struct
        {
            if (value == null || value == DBNull.Value)
            {
                if (targetType == typeof(T?))
                {
                    return null;
                }
                return default(T);
            }
            Type type = value.GetType();
            if (type == targetType)  //类型相同时
            {
                return (T)value;
            }
            return fun(value.ToString().Trim());
        }

        /// <summary>
        /// String转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToStringConvert(Type targetType, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            return value.ToString();
        }

        /// <summary>
        /// Byte转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToByteConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<byte>(Convert.ToByte, targetType, value);
        }

        /// <summary>
        /// SByte转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToSByteConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<sbyte>(Convert.ToSByte, targetType, value);
        }

        /// <summary>
        /// short转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToInt16Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<short>(Convert.ToInt16, targetType, value);
        }

        /// <summary>
        /// UInt16转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToUInt16Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<ushort>(Convert.ToUInt16, targetType, value);
        }

        /// <summary>
        /// Int32转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToInt32Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<int>(Convert.ToInt32, targetType, value);
        }

        /// <summary>
        /// UInt32转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToUInt32Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<uint>(Convert.ToUInt32, targetType, value);
        }

        /// <summary>
        /// Int64转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToInt64Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<long>(Convert.ToInt64, targetType, value);
        }

        /// <summary>
        /// UInt64转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToUInt64Convert(Type targetType, object value)
        {
            return ToValueTypeConvert<ulong>(Convert.ToUInt64, targetType, value);
        }

        /// <summary>
        /// Single转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToSingleConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<float>(Convert.ToSingle, targetType, value);
        }

        /// <summary>
        /// Double转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToDoubleConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<double>(Convert.ToDouble, targetType, value);
        }

        /// <summary>
        /// Decimal转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToDecimalConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<decimal>(Convert.ToDecimal, targetType, value);
        }

        /// <summary>
        /// Boolean转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToBooleanConvert(Type targetType, object value)
        {
            if (value != null && value is string)
            {
                string str = value.ToString().Trim().ToUpperInvariant();
                if (str == "TRUE" || str == "1" || str == "YES" || str == "Y" || str == "是")
                {
                    return true;
                }
                else if (str == "FALSE" || str == "0" || str == "NO" || str == "N" || str == "否")
                {
                    return false;
                }
            }
            return ToValueTypeConvert<bool>(Convert.ToBoolean, targetType, value);
        }

        /// <summary>
        /// Char转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToCharConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<char>(Convert.ToChar, targetType, value);
        }

        /// <summary>
        /// Guid转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToGuidConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<Guid>(Guid.Parse, targetType, value);
        }

        /// <summary>
        /// 日期时间转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToDateTimeConvert(Type targetType, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                if (targetType == typeof(DateTime?))
                {
                    return null;
                }
                return default(DateTime);
            }
            Type type = value.GetType();
            if (type.Equals(targetType))
            {
                return value;
            }
            if (type == typeof(string))
            {
                return Convert.ToDateTime(value.ToString().Trim());
            }
            else if (type == typeof(double))
            {
                return DateTime.FromOADate(Convert.ToDouble(value));
            }
            else
            {
                throw new InvalidCastException(string.Format("无法将类型 {0} 转换为类型{1}。", type.FullName, targetType.FullName));
            }
        }

        /// <summary>
        /// 时间间隔转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToTimeSpanConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<TimeSpan>(TimeSpan.Parse, targetType, value);
        }

        /// <summary>
        /// 世界时转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToDateTimeOffsetConvert(Type targetType, object value)
        {
            return ToValueTypeConvert<DateTimeOffset>(DateTimeOffset.Parse, targetType, value);
        }

        /// <summary>
        /// 枚举转换
        /// </summary>
        /// <param name="targetType">需要转换的目标类型</param>
        /// <param name="value">值</param>
        /// <returns>返回转换后的值</returns>
        private static object ToEnumConvert(Type targetType, object value)
        {
            if (value is string)
            {
                return Enum.Parse(targetType, value.ToString().Trim(), true);
            }
            else
            {
                if (targetType.IsGenericType && targetType.IsValueType &&
                    typeof(Nullable<>).IsAssignableFrom(targetType.GetGenericTypeDefinition()))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }
                return Enum.ToObject(targetType, value);
            }
        }

        #endregion
    }
}
