using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surpass.Utils.Security;

namespace Surpass.Utils.Test.Security
{
    [TestFixture]
    public class Md5UtilsTest
    {
        private const string test_Value = "!@#$%^&*()~<>*/+-|{}[]'<>,.abcdefghijkmlnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ中华人民共和国";
        private readonly Encoding test_Encoding = Encoding.UTF8;

        [Test]
        public void ToMd5ArrayTest()
        {
            Assert.True(Md5Utils.ToMd5Array("", test_Encoding) != null);
            Assert.True(Md5Utils.ToMd5Array(test_Value, test_Encoding) != null);           
        }

        [Test]
        public void ToMd5HexStringTest()
        {
            string value = Md5Utils.ToMd5HexString(test_Value, test_Encoding);
            Console.WriteLine(value);
            Assert.True(value != null);
        }

        [Test]
        public void ToMd5HexTest()
        {
            string value = Md5Utils.ToMd5Hex(test_Value, test_Encoding);
            Console.WriteLine(value);
            Assert.True(value != null);
        }

        [Test]
        public void ToMd5Test()
        {
            string value = Md5Utils.ToMd5(test_Value, test_Encoding);
            Console.WriteLine(value);
            Assert.True(value != null);
        }

        [Test]
        public void ToMd5HexGuidStringTest()
        {
            string value = Md5Utils.ToMd5HexGuidString(test_Value, test_Encoding);
            Console.WriteLine(value);
            Assert.True(value != null);
        }
    }
}
