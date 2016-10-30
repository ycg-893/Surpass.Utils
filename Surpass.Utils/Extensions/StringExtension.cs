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
        public static string LeftValue(this string value, int length)
        {
            value = ExceptionUtils.CheckNotNullAndWhiteSpace(value, nameof(value));
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
        public static string RightValue(this string value, int length)
        {
            value = ExceptionUtils.CheckNotNullAndWhiteSpace(value, nameof(value));
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
            value = ExceptionUtils.CheckNotNullAndWhiteSpace(value, nameof(value));
            if (value.Length > 1)
            {
                return string.Format("{0}{1}", value.Substring(0, 1).ToUpperInvariant(), value.Substring(1));
            }
            else
            {
                return value.ToUpperInvariant();
            }
        }


        #region ByteArray

        /// <summary>
        /// 获取默认格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToDefaultBytes(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }

        /// <summary>
        /// 获取 GB2312 格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToGB2312Bytes(this string value)
        {
            return Encoding.GetEncoding("GB2312").GetBytes(value);
        }

        /// <summary>
        /// 获取 UTF7 格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToUTF7Bytes(this string value)
        {
            return Encoding.UTF7.GetBytes(value);
        }

        /// <summary>
        /// 获取 UTF8 格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToUTF8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// 获取 UTF16 格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToUnicodeBytes(this string value)
        {
            return Encoding.Unicode.GetBytes(value);
        }

        /// <summary>
        /// 获取 UTF32 格式的编码字节数组
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static byte[] ToUTF32Bytes(this string value)
        {
            return Encoding.UTF32.GetBytes(value);
        }

        #endregion

        #region Hash

        /// <summary>
        /// 转换 Md5 的数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] ToMD5Array(this string value, Encoding encoding)
        {
            value = ExceptionUtils.CheckNotNullAndWhiteSpace(value, nameof(value));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            HashAlgorithm algorithm = HashAlgorithm.Create("MD5");
            byte[] bytes = encoding.GetBytes(value);
            return algorithm.ComputeHash(bytes);
        }

        /// <summary>
        /// 转换 Md5 的 Base64 编码值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMD5Base64(this string value, Encoding encoding)
        {
            return Convert.ToBase64String(value.ToMD5Array(encoding));
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值 例如“7F-2C-4A”
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMD5Hex(this string value, Encoding encoding)
        {
            return System.BitConverter.ToString(ToMD5Array(value, encoding));
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值后生成 Guid 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static Guid ToMD5HexGuid(this string value, Encoding encoding)
        {
            value = ToMD5Hex(value, encoding).Replace("-", "");
            return Guid.Parse(value);
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值后生成Guid 例如“0B5A7DE4-E6D1-23FD-D555-712175B9CE92”
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMD5HexGuidString(this string value, Encoding encoding)
        {
            return ToMD5HexGuid(value, encoding).ToString("D").ToUpperInvariant();
        }

        #endregion
    }
}
