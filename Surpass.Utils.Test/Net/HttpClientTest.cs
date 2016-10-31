using NUnit.Framework;
using Surpass.Utils.Net;
using Surpass.Utils.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test.Net
{
    [TestFixture]
    public class HttpClientTest
    {
        [Test]
        public void GetBaidu()
        {
            HttpClient client = new HttpClient();
            var str = client.GetString("https://www.baidu.com", null);
            Assert.True(!string.IsNullOrWhiteSpace(str));
        }
    }
}
