using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Security
{
    /// <summary>
    /// Md5帮助
    /// </summary>
    public static class Md5Utils
    {
        /// <summary>
        /// 转换 Md5 的数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] ToMd5Array(string value, Encoding encoding)
        {
            value = ExceptionUtils.CheckNotNull(value, nameof(value));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = encoding.GetBytes(value);
                return md5.ComputeHash(bytes);
            }
        }

        /// <summary>
        /// 转换 Md5 的 Base64 编码值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMd5Base64(string value, Encoding encoding)
        {
            return Convert.ToBase64String(ToMd5Array(value, encoding));
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值 例如“7F-2C-4A”
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMd5Hex(string value, Encoding encoding)
        {
            return System.BitConverter.ToString(ToMd5Array(value, encoding));
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值后生成 Guid 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static Guid ToMd5HexGuid(string value, Encoding encoding)
        {
            value = ToMd5Hex(value, encoding).Replace("-", "");
            return Guid.Parse(value);
        }

        /// <summary>
        /// 转换 Md5 的 十六进制值后生成Guid 例如“0B5A7DE4-E6D1-23FD-D555-712175B9CE92”
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMd5HexGuidString(string value, Encoding encoding)
        {
            return ToMd5HexGuid(value, encoding).ToString("D");
        }

        /// <summary>
        /// 转换 Md5 的 十六进制String
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMd5HexString(string value, Encoding encoding)
        {
            return ToMd5Hex(value, encoding).Replace("-", "");
        }

        /// <summary>
        /// 转换为普通的Md5
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ToMd5(string content, Encoding encoding)
        {
            byte[] buffer = ToMd5Array(content, encoding);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x2"));
            }
            return builder.ToString();
        }
       
    }
}
