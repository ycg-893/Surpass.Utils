using NUnit.Framework;
using Surpass.Utils.Security;
using Surpass.Utils.Test.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Test.Security
{
    [TestFixture]
    public class RsaUtilsTest
    {

        [Test]
        public void EncryptToDecryptTest()
        {

            var rsaPublicKey = Resources.TestRsaPublicKey.Trim();
            var rsaPirvateKey = Resources.TestRsaPraviteKey.Trim();

            //连续几个 Guid 会出现 Bug
            string source = Guid.NewGuid().ToString();
            Console.WriteLine("原文:" + source);

            string strEncrypt1 = RsaFromPkcs8Utils.Encrypt(source, rsaPublicKey, Encoding.UTF8);
            Console.WriteLine("加密后1:" + strEncrypt1);


            string strEncrypt2 = RsaFromPkcs8Utils.Encrypt(source, rsaPublicKey, Encoding.UTF8);
            Console.WriteLine("加密后2:" + strEncrypt2);


            string strDecrypt1 = RsaFromPkcs8Utils.Decrypt(strEncrypt1, rsaPirvateKey, Encoding.UTF8);
            Console.WriteLine("加密后1:" + strDecrypt1);
            Assert.True(source.Equals(strDecrypt1));

            string strDecrypt2 = RsaFromPkcs8Utils.Decrypt(strEncrypt2, rsaPirvateKey, Encoding.UTF8);
            Console.WriteLine("加密后2:" + strDecrypt2);
            Assert.True(source.Equals(strDecrypt2));
        }

        [Test]
        public void SignToVerify()
        {
            var rsaPublicKey = Resources.TestRsaPublicKey.Trim();
            var rsaPirvateKey = Resources.TestRsaPraviteKey.Trim();


            string source = Guid.NewGuid().ToString() + "-" +  Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
            Console.WriteLine("原文:" + source);

            string strSigned1 = RsaFromPkcs8Utils.Sign(source, rsaPirvateKey, Encoding.UTF8);
            Console.WriteLine("签名值1:" + strSigned1);


            string strSigned2 = RsaFromPkcs8Utils.Sign(source, rsaPirvateKey, Encoding.UTF8);
            Console.WriteLine("签名值2:" + strSigned2);


            bool signVerify1 = RsaFromPkcs8Utils.SignVerify(source, strSigned1, rsaPublicKey, Encoding.UTF8);
            Console.WriteLine("验签1:" + signVerify1);
            Assert.True(signVerify1);

            bool signVerify2 = RsaFromPkcs8Utils.SignVerify(source, strSigned2, rsaPublicKey, Encoding.UTF8);
            Console.WriteLine("验签2:" + signVerify2);
            Assert.True(signVerify2);
        }
    }
}
