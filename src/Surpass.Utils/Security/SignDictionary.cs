using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Security
{
    /// <summary>
    /// 签名字典
    /// </summary>
    public class SignDictionary : SortedDictionary<string, string>
    {
        /// <summary>
        /// 实例化 SignDictionary 类新实例
        /// </summary>
        public SignDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// 实例化 SignDictionary 类新实例
        /// </summary>
        /// <param name="dictionary">初始字典</param>
        public SignDictionary(IDictionary<string, string> dictionary)
            : base(dictionary, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// 获取Url连接字符
        /// </summary>
        /// <returns></returns>
        public string UrlLinkString()
        {
            return this.UrlLinkString(null, null);
        }

        /// <summary>
        /// 获取Url连接字符
        /// </summary>
        /// <param name="keyFunn"></param>
        /// <param name="valueFunc"></param>
        /// <returns></returns>
        public string UrlLinkString(Func<string, string> keyFunn, Func<string, string> valueFunc)
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in this)
            {
                if (!string.IsNullOrWhiteSpace(pair.Key)
                    && !string.IsNullOrWhiteSpace(pair.Value))
                {
                    string key = keyFunn != null ? keyFunn(pair.Key) : pair.Key;
                    string value = valueFunc != null ? valueFunc(pair.Value) : pair.Value;
                    builder.Append(key + "=" + value + "&");
                }
            }
            int length = builder.Length;
            if (length > 0)
            {
                builder.Remove(length - 1, 1);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="keyFunn"></param>
        /// <param name="valueFunc"></param>
        /// <returns></returns>
        public SignDictionary Clone(Func<string, string> keyFunn, Func<string, string> valueFunc)
        {
            SignDictionary dic = new SignDictionary();
            foreach (KeyValuePair<string, string> pair in this)
            {
                if (!string.IsNullOrWhiteSpace(pair.Key)
                    && !string.IsNullOrWhiteSpace(pair.Value))
                {
                    string key = keyFunn != null ? keyFunn(pair.Key) : pair.Key;
                    string value = valueFunc != null ? valueFunc(pair.Value) : pair.Value;
                    dic[key] = value;
                }
            }
            return dic;
        }
    }
}
