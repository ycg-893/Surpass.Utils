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
            Console.WriteLine(str + " -> " + value + " -> "  + str1);
            Assert.True(str == str1);

        }

        [Test]
        public void Test2()
        {
            string str = " ";          
            var value = UrlEncoderUtils.UrlEncode(str, null);           
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Console.WriteLine(str + " -> " + value + " -> " + str1);
            Assert.True(str == str1);

        }

        [Test]
        public void Test3()
        {
            string str = "http://www.abc.com";          
            var value = UrlEncoderUtils.UrlEncode(str, null);          
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Console.WriteLine(str + " -> " + value + " -> " + str1);
            Assert.True(str == str1);

        }

        [Test]
        public void Test4()
        {
            string str = "http://www.abc.com?a=中 国&b=abc";          
            var value = UrlEncoderUtils.UrlEncode(str, null);            
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Console.WriteLine(str + " -> " + value + " -> " + str1);
            Assert.True(str == str1);

        }

        [Test]
        public void Test5()
        {
            string str = "中";         
            var value = UrlEncoderUtils.UrlEncode(str, null);         
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Console.WriteLine(str + " -> " + value + " -> " + str1);
            Assert.True(str == str1);

        }

        [Test]
        public void Test6()
        {
            string str = "http://www.abc.com?a=++c";
            var value = UrlEncoderUtils.UrlEncode(str, null);
            var str1 = UrlEncoderUtils.UrlDecode(value, null);
            Console.WriteLine(str + " -> " + value + " -> " + str1);
            Assert.True(str == str1);

        }
    }
}
