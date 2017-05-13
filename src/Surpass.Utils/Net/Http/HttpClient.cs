using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Http 客户端
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// 默认 ContentType 类型
        /// </summary>
        public static string default_ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        /// 静态实例化
        /// </summary>
        static HttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidation);
        }

        private static bool RemoteCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }

        /// <summary>
        /// 实例化 HttpClient 类新实例
        /// </summary>
        public HttpClient()
        {
            this._Encoding = Encoding.UTF8;
            this.ProtocolVersion = HttpVersion.Version11;
            this.ContentType = default_ContentType;
        }

        /// <summary>
        /// 获取或设置Cookie容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding _Encoding;

        /// <summary>
        /// 获取或设置编码
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this._Encoding;
            }
            set
            {
                this._Encoding = ExceptionUtils.CheckNotNull(value, nameof(value));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<X509Certificate> _Certificates;

        /// <summary>
        /// 获取或设置证书
        /// </summary>
        public IList<X509Certificate> Certificates
        {
            get
            {
                if (this._Certificates == null)
                {
                    this._Certificates = new List<X509Certificate>();
                }
                return this._Certificates;
            }
        }

        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Http 版本
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        /// 获取或设置 Accept HTTP 标头的值。
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示请求是否应跟随重定向响应。
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// 获取或设置 Content-type HTTP 标头的值。
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 获取或设置写入或读取流时的超时（以毫秒为单位）;默认值为 300,000 毫秒（5 分钟）。
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 300000;

        /// <summary>
        /// 获取或设置 System.Net.HttpWebRequest.GetResponse 和 System.Net.HttpWebRequest.GetRequestStream
        ///     方法的超时值（以毫秒为单位）。默认值是 100,000 毫秒（100 秒）
        /// </summary>
        public int Timeout { get; set; } = 100000;

        /// <summary>
        /// 获取或设置 User-agent HTTP 标头的值。
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 请求字符值
        /// </summary>
        /// <param name="requestDictionary"></param>
        /// <param name="isUrlEncode">是否编码</param>
        /// <returns></returns>
        private string RequestString(IDictionary<string, string> requestDictionary, bool isUrlEncode)
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
                        builder.Append(isUrlEncode ? UrlEncoderUtils.UrlEncode(keyValue.Value, this.Encoding) : keyValue.Value);
                        count++;
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 设置请求
        /// </summary>
        /// <param name="request"></param>      
        private void SetRequest(HttpWebRequest request)
        {
            string contentType = this.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = default_ContentType;
            }
            request.ContentType = contentType + ";charset=" + this.Encoding.WebName;
            request.Accept = this.Accept;
            request.AllowAutoRedirect = this.AllowAutoRedirect;
            request.Credentials = this.Credentials;
            request.ReadWriteTimeout = this.ReadWriteTimeout;
            request.Timeout = this.Timeout;
            request.UserAgent = this.UserAgent;
            var certificates = this._Certificates;
            if (certificates != null && certificates.Count > 0)
            {
                request.ClientCertificates.AddRange(certificates.ToArray());
            }
            var cookieContainer = this.CookieContainer;
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }
        }

        /// <summary>
        /// Http Get 字符异步响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>           
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri requestUrl)
        {
            return await Task.Run<string>(() =>
            {
                return GetString(requestUrl);
            });
        }

        /// <summary>
        /// Http Get 字符响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>           
        /// <returns></returns>
        public string GetString(Uri requestUrl)
        {
            var response = Get(requestUrl);
            return response.ResponseStringResult(this.Encoding);
        }

        /// <summary>
        /// Http Get 异步字符响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public async Task<string> GetStringAsync(string requestUrl,
            IDictionary<string, string> requestDictionary)
        {
            return await Task.Run<string>(() =>
            {
                return GetString(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Get 字符响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public string GetString(string requestUrl,
            IDictionary<string, string> requestDictionary)
        {
            var response = Get(requestUrl, requestDictionary);
            return response.ResponseStringResult(this.Encoding);
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>
        /// <returns></returns>
        public async Task<HttpWebResponse> GetAsync(string requestUrl,
           IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(requestUrl, nameof(requestUrl));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Get(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>      
        /// <returns></returns>
        public HttpWebResponse Get(string requestUrl, IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(requestUrl, nameof(requestUrl));
            var requestData = this.RequestString(requestDictionary, true);
            requestUrl = requestUrl.Trim();
            Uri url = new Uri(requestUrl + (requestData.Length == 0 || requestUrl.Substring(requestUrl.Length - 1) == "?" ? "" : "?") + requestData);
            return Get(url);
        }

        /// <summary>
        /// Http Get 异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>     
        /// <returns></returns>
        public async Task<HttpWebResponse> GetAsync(Uri requestUrl)
        {
            ExceptionUtils.CheckNotNull(requestUrl, nameof(requestUrl));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Get(requestUrl);
            });
        }

        /// <summary>
        /// Http Post 字符响应异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public async Task<string> PostStringAsync(string requestUrl, IDictionary<string, string> requestDictionary)
        {
            return await Task.Run<string>(() =>
            {
                return PostString(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Post 字符响应异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri requestUrl, IDictionary<string, string> requestDictionary)
        {
            return await Task.Run<string>(() =>
            {
                return PostString(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Post 字符响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public string PostString(string requestUrl, IDictionary<string, string> requestDictionary)
        {
            var response = Post(requestUrl, requestDictionary);
            return response.ResponseStringResult(this.Encoding);
        }

        /// <summary>
        /// Http Post 字符响应请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public string PostString(Uri requestUrl, IDictionary<string, string> requestDictionary)
        {
            var response = Post(requestUrl, requestDictionary);
            return response.ResponseStringResult(this.Encoding);
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public async Task<HttpWebResponse> PostAsync(string requestUrl, IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(requestUrl, nameof(requestUrl));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Post(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public HttpWebResponse Post(string requestUrl, IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(requestUrl, nameof(requestUrl));
            return Post(new Uri(requestUrl), requestDictionary);
        }

        /// <summary>
        /// Http Post 异步请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public async Task<HttpWebResponse> PostAsync(Uri requestUrl, IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNull(requestUrl, nameof(requestUrl));
            return await Task.Run<HttpWebResponse>(() =>
            {
                return Post(requestUrl, requestDictionary);
            });
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>      
        /// <returns></returns>
        public HttpWebResponse Get(Uri requestUrl)
        {
            ExceptionUtils.CheckNotNull(requestUrl, nameof(requestUrl));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.ProtocolVersion = this.ProtocolVersion;
            request.Method = "GET";
            SetRequest(request);
            var response = (HttpWebResponse)request.GetResponse();
            var cookieContainer = this.CookieContainer;
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }
            return response;
        }

        /// <summary>
        /// Http Post 请求
        /// </summary>
        /// <param name="requestUrl">requestUrl地址</param>
        /// <param name="requestDictionary">请求字典</param>       
        /// <returns></returns>
        public HttpWebResponse Post(Uri requestUrl, IDictionary<string, string> requestDictionary)
        {
            ExceptionUtils.CheckNotNull(requestUrl, nameof(requestUrl));
            var requestData = RequestString(requestDictionary, false);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.ProtocolVersion = this.ProtocolVersion;
            request.Method = "POST";
            SetRequest(request);
            byte[] bytes = this.Encoding.GetBytes(requestData);
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
            var cookieContainer = this.CookieContainer;
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }
            return response;
        }
    }
}
