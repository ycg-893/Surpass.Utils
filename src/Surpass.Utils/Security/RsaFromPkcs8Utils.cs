using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Surpass.Utils.Security
{
    /// <summary>
    /// Pkcs8 帮助,目前仅支持1024位
    /// </summary>
    public static class RsaFromPkcs8Utils
    {
        /// <summary>
        /// 签名 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="privateKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sign(string content, string privateKey, Encoding encoding)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(content, nameof(content));
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(privateKey, nameof(privateKey));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            byte[] Data = encoding.GetBytes(content);
            using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
            {
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    byte[] signData = rsa.SignData(Data, sha);
                    return Convert.ToBase64String(signData);
                }
            }
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="signedString"></param>
        /// <param name="publicKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool SignVerify(string content, string signedString, string publicKey, Encoding encoding)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(content, nameof(content));
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(signedString, nameof(signedString));
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(publicKey, nameof(publicKey));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            byte[] Data = encoding.GetBytes(content);
            byte[] data = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            using (RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider())
            {
                rsaPub.ImportParameters(paraPub);
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    return rsaPub.VerifyData(Data, sha, data);
                }
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string Encrypt(string content, string publicKey, Encoding encoding)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(content, nameof(content));
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(publicKey, nameof(publicKey));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            byte[] dataToEncrypt = encoding.GetBytes(content);
            return Encrypt(dataToEncrypt, publicKey);
        }


        /// <summary>
        /// 解密  
        /// </summary>
        /// <param name="content"></param>
        /// <param name="privateKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(string content, string privateKey, Encoding encoding)
        {
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(content, nameof(content));
            ExceptionUtils.CheckNotNullOrNotWhiteSpace(privateKey, nameof(privateKey));
            ExceptionUtils.CheckNotNull(encoding, nameof(encoding));
            byte[] dataToDecrypt = Convert.FromBase64String(content);
            StringBuilder result = new StringBuilder();
            for (int j = 0; j < dataToDecrypt.Length / 128; j++)
            {
                byte[] buf = new byte[128];
                for (int i = 0; i < 128; i++)
                {
                    buf[i] = dataToDecrypt[i + 128 * j];
                }
                result.Append(Decrypt(buf, privateKey, encoding));
            }
            return result.ToString();
        }

        #region 内部方法  

        private static string Encrypt(byte[] data, string publicKey)
        {
            RSACryptoServiceProvider rsa = DecodePemPublicKey(publicKey);
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                byte[] result = rsa.Encrypt(data, false);
                return Convert.ToBase64String(result);
            }
        }

        private static string Decrypt(byte[] data, string privateKey, Encoding encoding)
        {
            using (RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey))
            {
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    byte[] source = rsa.Decrypt(data, false);
                    char[] asciiChars = new char[encoding.GetCharCount(source, 0, source.Length)];
                    encoding.GetChars(source, 0, source.Length, asciiChars, 0);
                    return new string(asciiChars);
                }
            }
        }

        private static RSACryptoServiceProvider DecodePemPublicKey(String pemstr)
        {
            byte[] pkcs8publickkey = Convert.FromBase64String(pemstr);
            var publicKey = DecodeRSAPublicKey(pkcs8publickkey);
            if (publicKey == null)
            {
                ExceptionUtils.ThrowFormatException("公钥格式不正确，只支持1024位生成的pkcs8公钥");
            }
            return publicKey;
        }

        private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
        {
            byte[] pkcs8privatekey = Convert.FromBase64String(pemstr);
            var privateKey = DecodeRSAPrivateKey(pkcs8privatekey);
            if (privateKey == null)
            {
                ExceptionUtils.ThrowFormatException("公钥格式不正确，只支持1024位生成的pkcs8私钥");
            }
            return privateKey;
        }

        private static byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }
        
        private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"             
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------  
            using (MemoryStream ms = new MemoryStream(publickey))
            {
                using (BinaryReader read = new BinaryReader(ms))
                {    //wrap Memory Stream with BinaryReader for easy reading  
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = read.ReadUInt16();
                    if (twobytes == 0x8130)
                    { //data read as little endian order (actual data order for Sequence is 30 81)  

                        read.ReadByte();    //advance 1 byte  
                    }
                    else if (twobytes == 0x8230)
                    {
                        read.ReadInt16();
                    }  //advance 2 bytes  
                    else
                    {
                        return null;
                    }
                    seq = read.ReadBytes(15);       //read the Sequence OID  
                    if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct 
                    {
                        return null;
                    }
                    twobytes = read.ReadUInt16();
                    if (twobytes == 0x8103)
                    {//data read as little endian order (actual data order for Bit String is 03 81)  
                        read.ReadByte();
                    }   //advance 1 byte  
                    else if (twobytes == 0x8203)
                    {
                        read.ReadInt16();   //advance 2 bytes  
                    }
                    else
                    {
                        return null;
                    }
                    bt = read.ReadByte();
                    if (bt != 0x00)     //expect null byte next  
                    {
                        return null;
                    }
                    twobytes = read.ReadUInt16();
                    if (twobytes == 0x8130)
                    { //data read as little endian order (actual data order for Sequence is 30 81)  
                        read.ReadByte();
                    }  //advance 1 byte  
                    else if (twobytes == 0x8230)
                    {
                        read.ReadInt16();   //advance 2 bytes  
                    }
                    else
                    {
                        return null;
                    }
                    twobytes = read.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                    {//data read as little endian order (actual data order for Integer is 02 81)  
                        lowbyte = read.ReadByte();
                    }  // read next bytes which is bytes in modulus  
                    else if (twobytes == 0x8202)
                    {
                        highbyte = read.ReadByte(); //advance 2 bytes  
                        lowbyte = read.ReadByte();
                    }
                    else
                    {
                        return null;
                    }
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order  
                    int modsize = BitConverter.ToInt32(modint, 0);
                    byte firstbyte = read.ReadByte();
                    read.BaseStream.Seek(-1, SeekOrigin.Current);
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it  
                        read.ReadByte();    //skip this null byte  
                        modsize -= 1;   //reduce modulus buffer size by 1  
                    }
                    byte[] modulus = read.ReadBytes(modsize);   //read the modulus bytes  
                    if (read.ReadByte() != 0x02)
                    {          //expect an Integer for the exponent data  
                        ExceptionUtils.ThrowFormatException("Rsa PublicKey 不是有效的格式。");
                    }
                    int expbytes = (int)read.ReadByte();        // should only need one byte for actual exponent data (for all useful values)  
                    byte[] exponent = read.ReadBytes(expbytes);
                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----  
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSAParameters rsaKeyInfo = new RSAParameters();
                    rsaKeyInfo.Modulus = modulus;
                    rsaKeyInfo.Exponent = exponent;
                    RSA.ImportParameters(rsaKeyInfo);
                    return RSA;
                }
            }
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;
            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------  
            using (MemoryStream ms = new MemoryStream(privkey))
            {
                using (BinaryReader read = new BinaryReader(ms))
                {    //wrap Memory Stream with BinaryReader for easy reading  
                    byte bt = 0;
                    ushort twobytes = 0;
                    int elems = 0;
                    twobytes = read.ReadUInt16();
                    if (twobytes == 0x8130)
                    { //data read as little endian order (actual data order for Sequence is 30 81)  
                        read.ReadByte();
                    }   //advance 1 byte  
                    else if (twobytes == 0x8230)
                    {
                        read.ReadInt16();  //advance 2 bytes  
                    }
                    else
                    {
                        return null;
                    }
                    twobytes = read.ReadUInt16();
                    if (twobytes != 0x0102)    //version number
                    {
                        return null;
                    }
                    bt = read.ReadByte();
                    if (bt != 0x00)
                    {
                        return null;
                    }
                    //------  all private key components are Integer sequences ----  
                    elems = GetIntegerSize(read);
                    MODULUS = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    E = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    D = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    P = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    Q = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    DP = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    DQ = read.ReadBytes(elems);

                    elems = GetIntegerSize(read);
                    IQ = read.ReadBytes(elems);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----  
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSAParameters RSAparams = new RSAParameters();
                    RSAparams.Modulus = MODULUS;
                    RSAparams.Exponent = E;
                    RSAparams.D = D;
                    RSAparams.P = P;
                    RSAparams.Q = Q;
                    RSAparams.DP = DP;
                    RSAparams.DQ = DQ;
                    RSAparams.InverseQ = IQ;
                    RSA.ImportParameters(RSAparams);
                    return RSA;
                }
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)        //expect integer  
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte  
            else
            if (bt == 0x82)
            {
                highbyte = binr.ReadByte();    // data size in next 2 bytes  
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;        // we already have the data size  
            }
            while (binr.ReadByte() == 0x00)
            {    //remove high order zeros in data  
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte  
            return count;
        }

        #endregion

        #region 解析.net 生成的Pem  
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new NotSupportedException("格式不正确，只支持1024位的公钥");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new NotSupportedException("格式不正确，只支持1024位的私钥");
            }
            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141  
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148  
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += keyData[index + 1] == 64 ? 2 : 3;//279  
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += keyData[index + 1] == 64 ? 2 : 3;//346  
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += keyData[index + 1] == 64 ? 2 : 3;//412/413  
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += keyData[index + 1] == 64 ? 2 : 3;//479/480  
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += keyData[index + 1] == 64 ? 2 : 3;//545/546  
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion

    }
}
