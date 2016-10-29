using Surpass.Utils.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Exceptions
{
    /// <summary>
    /// 字符空白引发的异常
    /// </summary>
    public class ArgumentWhiteSpaceException : ArgumentException
    {

        /// <summary>
        /// 实例化 ArgumentWhiteSpaceException 类新实例
        /// </summary>
        public ArgumentWhiteSpaceException(string paramName)
            : base((paramName == null ? string.Format(Resources.ArgumentWhiteSpaceErrorMessage, "") : 
                  string.Format(Resources.ArgumentWhiteSpaceErrorMessage, " " + paramName + " 的")), paramName)
        {

        }

        /// <summary>
        /// 实例化 ArgumentWhiteSpaceException 类新实例
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="message">消息</param>
        public ArgumentWhiteSpaceException(string paramName, string message)
            : base(message, paramName)
        {

        }
    }
}
