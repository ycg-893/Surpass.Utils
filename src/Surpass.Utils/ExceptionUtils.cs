using Surpass.Utils.Exceptions;
using Surpass.Utils.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils
{
    /// <summary>
    /// 异常帮助
    /// </summary>
    public static class ExceptionUtils
    {      
        /// <summary>
        /// 检查非空
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static T CheckNotNull<T>(T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name, name == null ? string.Format(Resources.ArgumentNullErrorMessage, "")
                    : string.Format(Resources.ArgumentNullErrorMessage, " " + name + " 的"));
            }
            return value;
        }

        /// <summary>
        /// 检查字符空值或空白字符
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string CheckNotNullOrNotWhiteSpace(string value, string name)
        {
            CheckNotNull(value, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentWhiteSpaceException(name);
            }
            return value;
        }

        /// <summary>
        /// 数据格式不正确异常
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static FormatException ThrowFormatException(string message)
        {
            throw new FormatException(message);
        }

        /// <summary>
        /// 由于不支持而抛出的异常
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static NotSupportedException ThrowNotSupportedException(string message)
        {
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// 抛出验证性异常
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValidationException ThrowValidationException(string message)
        {
            throw new ValidationException(message);
        }
    }
}
