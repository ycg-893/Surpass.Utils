using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 字符窜扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 全部字母正则
        /// </summary>
        public const string ALLLetterPattern = "^[a-zA-Z]+[a-zA-z]$+";

        /// <summary>
        /// 全部数字正则
        /// </summary>
        public const string ALLNumberPattern = "^[0-9]+[0-9]$+";

        /// <summary>
        /// 全部是字母或数字正则
        /// </summary>
        public const string ALLNumberOrLetterPattern = "^[a-zA-Z0-9]+[a-zA-z0-9]$+";

        /// <summary>
        /// 中文表达式
        /// </summary>
        public const string ChinesePattern = "[\u4E00 - \u9FA0]+";

        /// <summary>
        /// 是否是空白字符
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsWhiteChar(this char value)
        {
            return value == ' ' || value == '\t' || value == '\r' || value == '\n';
        }

        /// <summary>
        /// 使用固定区域，忽略大小写比较两个字符窜是否相等
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="target">目标</param>
        /// <returns></returns>
        public static bool IgnoreCaseEquals(this string source, string target)
        {
            return string.Compare(source, target, true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsContainsChinese(this string value)
        {
            return Regex.IsMatch(value, @"[\u4E00 - \u9FA0]+");
        }

        /// <summary>
        /// 是否包含空格
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsContainsBlank(this string value)
        {
            if (value == null)
            {
                return false;
            }
            return value.Contains(" ");
        }

        /// <summary>
        /// 是否是英文字母
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsEnglishLetter(this string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!StringExtension.IsEnglishLetter(value[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 是否是英文字母
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsEnglishLetter(this char value)
        {
            return ((value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z'));
        }

        /// <summary>
        /// 是否全部为数字
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsALLNumber(this string value)
        {
            return Regex.IsMatch(value, StringExtension.ALLNumberPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否全部为数字与字母的组合
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsALLNumberOrLetter(this string value)
        {
            return Regex.IsMatch(value, StringExtension.ALLNumberOrLetterPattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 通过正则替换(默认为不区分大小写,自动转义)
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newdValue">新值</param>
        /// <param name="regexOptions">选项</param>
        /// <returns></returns>
        public static string RegexReplace(this string source, string oldValue, string newdValue,
            RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }
            if (newdValue == null)
            {
                throw new ArgumentNullException("newdValue");
            }
            oldValue = oldValue
                .Replace("$", @"\$")
                .Replace("(", @"\(")
                .Replace(")", @"\)")
                .Replace("*", @"\*")
                .Replace("+", @"\+")
                .Replace(".", @"\.")
                .Replace("[", @"\[")
                .Replace("]", @"\]")
                .Replace("?", @"\?")
                .Replace("^", @"\^")
                .Replace("|", @"\|")
                .Replace("{", @"\{")
                .Replace("}", @"\}")
                .Replace("\"", "\"\"");
            return Regex.Replace(source, string.Format(@"{0}", oldValue), newdValue, regexOptions);
        }

        /// <summary>
        /// 转换为目标值(若字符窜为 null 或 空白字符时返回类型的默认值)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static T ToTargetValue<T>(this string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }
            else
            {
                return (T)DataConvertUtils.TargetValueConvert(typeof(T), value);
            }
        }

        /// <summary>
        /// 尝试字符窜转换为目标值，返回 true 表示转成功 result 等于转换后的值，若 false 时，result 等于类型默认值(若字符窜为 null 或 空白字符时，转换成功，并返回类型的默认值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse<T>(this string value, out T result) where T : struct
        {
            result = default(T);
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            else
            {
                try
                {
                    result = (T)DataConvertUtils.TargetValueConvert(typeof(T), value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 从左起根据长度截断字符字。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string Left(this string value, int length)
        {
            if (value == null)
            {
                return value;
            }
            if (length >= value.Length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, length);
            }
        }

        /// <summary>
        /// 从右起根据长度截断字符字。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                return value;
            }
            if (length >= value.Length)
            {
                return value;
            }
            else
            {
                return value.Substring(value.Length - length, length);
            }
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string Reverse(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            StringBuilder str = new StringBuilder(value.Length);
            for (int index = value.Length - 1; index >= 0; index--)
            {
                str.Append(value[index]);
            }
            return str.ToString();

        }

        /// <summary>
        /// 首字母大写转换
        /// </summary>
        /// <param name="value">名称</param>
        /// <returns></returns>
        public static string FirstLetterUpperCase(this string value)
        {
            if (value == null)
            {
                return value;
            }
            if (value.Length > 1)
            {
                return string.Format("{0}{1}", value.Substring(0, 1).ToUpperInvariant(), value.Substring(1));
            }
            else
            {
                return value.ToUpperInvariant();
            }
        }

        /// <summary>
        /// 返回的字符串数组包含此字符串中的子字符串（由指定字符串数组的元素分隔）。参数指定是否返回空数组元素。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="separator">分隔此字符串中的子字符串的字符串数组、不包含分隔符的空数组或 null。</param>
        /// <returns></returns>
        public static string[] Split(this string value, string separator)
        {
            return Split(value, separator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 返回的字符串数组包含此字符串中的子字符串（由指定字符串数组的元素分隔）。参数指定是否返回空数组元素。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="separator">分隔此字符串中的子字符串的字符串数组、不包含分隔符的空数组或 null。</param>
        ///  <param name="options">要省略返回的数组中的空数组元素，则为 System.StringSplitOptions.RemoveEmptyEntries；要包含返回的数组中的空数组元素，则为 System.StringSplitOptions.None。</param>
        /// <returns></returns>
        public static string[] Split(this string value, string separator, StringSplitOptions options)
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            ExceptionUtils.CheckNotNull(separator, nameof(separator));
            return value.Split(new string[] { separator }, options);
        }

        /// <summary>
        /// 返回的字符串数组包含此字符串中的子字符串（由指定字符串数组的元素分隔）。参数指定是否返回空数组元素。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="separator">分隔此字符串中的子字符串的字符串数组、不包含分隔符的空数组或 null。</param>
        /// <returns></returns>
        public static string[] Split(this string value, char separator)
        {
            return Split(value, separator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 返回的字符串数组包含此字符串中的子字符串（由指定字符串数组的元素分隔）。参数指定是否返回空数组元素。
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="separator">分隔此字符串中的子字符串的字符串数组、不包含分隔符的空数组或 null。</param>
        ///  <param name="options">要省略返回的数组中的空数组元素，则为 System.StringSplitOptions.RemoveEmptyEntries；要包含返回的数组中的空数组元素，则为 System.StringSplitOptions.None。</param>
        /// <returns></returns>
        public static string[] Split(this string value, char separator, StringSplitOptions options)
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            ExceptionUtils.CheckNotNull(separator, nameof(separator));
            return value.Split(new char[] { separator }, options);
        }

        /// <summary>
        /// 使用 <see cref="Environment.NewLine"/> 分隔出新行 
        /// </summary>
        /// <param name="str"></param>       
        /// <returns></returns>
        public static string[] SplitToLines(this string str)
        {
            return Split(str, Environment.NewLine);
        }

        /// <summary>
        /// 使用 <see cref="Environment.NewLine"/> 分隔出新行
        /// </summary>
        /// <param name="str"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return Split(str, Environment.NewLine, options);
        }

        /// <summary>
        /// 转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">类型</typeparam>
        /// <param name="value">值</param>     
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        /// <summary>
        /// 转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="ignoreCase">true 为忽略大小写；false 为考虑大小写。</param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value, bool ignoreCase) where TEnum : struct
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }

        /// <summary>
        /// 转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="result">输出类型</param>
        /// <returns></returns>
        public static bool TryEnumParse<TEnum>(this string value, TEnum result) where TEnum : struct
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            return Enum.TryParse<TEnum>(value, out result);
        }

        /// <summary>
        /// 转换为枚举类型
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="ignoreCase">是否区分大小写</param>
        /// <param name="result">输出类型</param>
        /// <returns></returns>
        public static bool TryEnumParse<TEnum>(this string value, bool ignoreCase, TEnum result) where TEnum : struct
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            return Enum.TryParse<TEnum>(value, ignoreCase, out result);
        }

        /// <summary>
        /// 转为骆锋命名，即首个字母为小写
        /// </summary>
        /// <param name="value">值</param>      
        /// <returns></returns>
        public static string ToCamelCase(this string value)
        {
            return ToCamelCase(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 转为骆锋命名，即首个字母为小写
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="culture">区域</param>
        /// <returns></returns>
        public static string ToCamelCase(this string value, CultureInfo culture)
        {
            ExceptionUtils.CheckNotNull(culture, nameof(culture));
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            if (value.Length == 1)
            {
                return value.ToLower(culture);
            }
            return char.ToLower(value[0], culture) + value.Substring(1);
        }

        /// <summary>
        /// 转为帕斯卡命名，即首个字母为大写
        /// </summary>
        /// <param name="value">值</param>      
        /// <returns></returns>
        public static string ToPascalCase(this string value)
        {
            return ToPascalCase(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 转为帕斯卡命名，即首个字母为大写
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="culture">区域</param>
        /// <returns></returns>
        public static string ToPascalCase(this string value, CultureInfo culture)
        {
            ExceptionUtils.CheckNotNull(culture, nameof(culture));
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            if (value.Length == 1)
            {
                return value.ToUpper(culture);
            }
            return char.ToUpper(value[0], culture) + value.Substring(1);
        }

    }
}
