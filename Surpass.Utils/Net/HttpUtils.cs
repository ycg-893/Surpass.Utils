using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Net
{
    class HttpUtils
    {
        static HttpUtils()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        /// <summary>
        /// 获取请求值
        /// </summary>
        /// <param name="requestDictionary"></param>
        /// <param name="isUrlEncode">是否编码</param>
        /// <returns></returns>
        private static string RequestString(IDictionary<string, string> requestDictionary, bool isUrlEncode)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;
            if (requestDictionary != null && requestDictionary.Count > 0)
            {
                foreach (var keyValue in requestDictionary)
                {
                    if (!string.IsNullOrWhiteSpace(keyValue.Value)
                        && !string.IsNullOrWhiteSpace(keyValue.Key))
                    {
                        if (count > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(keyValue.Key);
                        builder.Append("=");
                        //builder.Append(isUrlEncode ? System.Web.HttpUtility.UrlEncode(keyValue.Value) : keyValue.Value);
                        count++;
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// http 响应结果
        /// </summary>
        /// <param name="response">响应</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string HttpStringResponseResult(HttpWebResponse response,
            Encoding encoding)
        {
            ExceptionUtils.CheckNotNull(response, nameof(response));
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            using (Stream myResponseStream = response.GetResponseStream())
            {
                using (var myStreamReader = new StreamReader(myResponseStream, encoding))
                {
                    return myStreamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> PostAsync(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Post(url, requestDictionary, encoding, cookieContainer, certificate);
            });
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            return await Task.Run<string>(() =>
            {
                return PostString(url, requestDictionary, encoding, cookieContainer, certificate);
            });
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static string PostString(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            var response = Post(url, requestDictionary, encoding, cookieContainer, certificate);
            return HttpStringResponseResult(response, encoding);
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static HttpWebResponse Post(string url, IDictionary<string, string> requestDictionary,
            Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            var requestData = RequestString(requestDictionary, false);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            SetRequest(request, encoding, cookieContainer, certificate);
            byte[] bytes = encoding.GetBytes(requestData);
            request.ContentLength = bytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                if (bytes.Length > 0)
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Flush();
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }
            return response;
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url,
            IDictionary<string, string> requestDictionary, Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            return await Task.Run<string>(() =>
            {
                return GetString(url, requestDictionary, encoding, cookieContainer, certificate);
            });
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> GetAsync(string url,
            IDictionary<string, string> requestDictionary, Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Get(url, requestDictionary, encoding, cookieContainer, certificate);
            });
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static string GetString(string url,
            IDictionary<string, string> requestDictionary, Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            var response = Get(url, requestDictionary, encoding, cookieContainer, certificate);
            return HttpStringResponseResult(response, encoding);
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <param name="encoding">编码</param>
        /// <param name="cookieContainer">cookie</param>
        /// <param name="certificate">证书</param>
        /// <returns></returns>
        public static HttpWebResponse Get(string url,
            IDictionary<string, string> requestDictionary, Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            ExceptionUtils.CheckNotNullAndWhiteSpace(url, nameof(url));
            var requestData = RequestString(requestDictionary, true);
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create(url + (requestData.Length == 0 ? "" : "?") + requestData);
            request.Method = "GET";
            SetRequest(request, encoding, cookieContainer, certificate);
            var response = (HttpWebResponse)request.GetResponse();
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }
            return response;
        }

        private static void SetRequest(HttpWebRequest request, Encoding encoding = null,
            CookieContainer cookieContainer = null,
            X509Certificate certificate = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            request.ContentType = "application/x-www-form-urlencoded;charset=" + encoding.WebName;
            if (certificate != null)
            {
                request.ClientCertificates.Add(certificate);
            }
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }
        }
    }
}
