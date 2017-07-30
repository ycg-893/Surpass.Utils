using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Net.Http
{
    /// <summary>
    /// Http 帮助
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// http 异步字符响应结果
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static Task<string> ResponseStringResultAsync(this HttpWebResponse response,
            Encoding encoding = null)
        {
            return TaskUtils.Run<string>(() =>
            {
                return ResponseStringResult(response, encoding);
            });
        }

        /// <summary>
        /// http 字符响应结果
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ResponseStringResult(this HttpWebResponse response,
            Encoding encoding = null)
        {
            try
            {
                ExceptionUtils.CheckNotNull(response, nameof(response));
                if (string.IsNullOrWhiteSpace(response.ContentEncoding))
                {
                    if (encoding == null)
                    {
                        encoding = Encoding.UTF8;
                    }
                }
                else
                {
                    encoding = Encoding.GetEncoding(response.ContentEncoding);
                }
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    using (var myStreamReader = new StreamReader(myResponseStream, encoding))
                    {
                        return myStreamReader.ReadToEnd();
                    }
                }
            }
            finally
            {
                response.Close();
            }
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>  
        /// <param name="encoding">编码</param>     
        /// <returns></returns>
        public static Task<HttpWebResponse> PostAsync(string url,
            IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            return TaskUtils.Run<HttpWebResponse>(() =>
            {
                return Post(url, requestDictionary, encoding);
            });
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>   
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static Task<string> PostStringAsync(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            return TaskUtils.Run<string>(() =>
            {
                return PostString(url, requestDictionary, encoding);
            });
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>  
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static string PostString(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            HttpClient client = new HttpClient();
            if (encoding != null)
            {
                client.Encoding = encoding;
            }
            var response = client.Post(url, requestDictionary);
            return response.ResponseStringResult(client.Encoding);
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>     
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static HttpWebResponse Post(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            HttpClient client = new HttpClient();
            if (encoding != null)
            {
                client.Encoding = encoding;
            }
            return client.Post(url, requestDictionary);
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>      
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static Task<string> GetStringAsync(string url,
            IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            return TaskUtils.Run<string>(() =>
            {
                return GetString(url, requestDictionary, encoding);
            });
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>      
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static Task<HttpWebResponse> GetAsync(string url,
            IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            return TaskUtils.Run<HttpWebResponse>(() =>
            {
                return Get(url, requestDictionary, encoding);
            });
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>     
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static string GetString(string url,
            IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            HttpClient client = new HttpClient();
            if (encoding != null)
            {
                client.Encoding = encoding;
            }
            var response = client.Get(url, requestDictionary);
            return response.ResponseStringResult(client.Encoding);
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>   
        /// <param name="encoding">编码</param>   
        /// <returns></returns>
        public static HttpWebResponse Get(string url,
            IDictionary<string, string> requestDictionary,
            Encoding encoding = null)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(url, nameof(url));
            HttpClient client = new HttpClient();
            if (encoding != null)
            {
                client.Encoding = encoding;
            }
            return client.Get(url, requestDictionary);
        }
    }
}
