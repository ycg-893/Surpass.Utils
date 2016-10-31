using NUnit.Framework;
using Surpass.Utils.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test.Net
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class UrlEncoderUtilsTest
    {
        [Test]
        public void Test1()
        {
            string str = "中华人民共和国";
            var value = UrlEncoderUtils.UrlEncode(str, null);
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Assert.True(str == str1);

        }
    }
}
