using NUnit.Framework;
using Surpass.Utils.Net.Http;

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
