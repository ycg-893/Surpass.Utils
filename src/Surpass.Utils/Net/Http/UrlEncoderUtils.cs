using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Surpass.Utils.Net.Http
{
    /// <summary>
    /// Url 编码帮助，基ms自带修改，实现标准的相关编解码，处理掉 + / : ? 等相关问题
    /// </summary>
    public static class UrlEncoderUtils
    {
        /// <summary>
        /// 十六进制转十进制
        /// </summary>
        /// <param name="h">值</param>
        /// <returns></returns>
        private static int HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
            {
                return h - '0';
            }
            else if (h >= 'a' && h <= 'f')
            {
                return h - 'a' + 10;
            }
            else if (h >= 'A' && h <= 'F')
            {
                return h - 'A' + 10;
            }
            return -1;
        }

        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <param name="n">值</param>
        /// <returns></returns>
        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        /// <summary>
        /// 是否是安全字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static bool IsUrlSafeChar(char ch)
        {
            if ((((ch < 'a') || (ch > 'z')) && ((ch < 'A') || (ch > 'Z'))) && ((ch < '0') || (ch > '9')))
            {
                switch (ch)
                {
                    case '(':
                    case ')':
                    case '*':
                    case '-':
                    case '.':
                    case '!':
                    case ':':
                    case '/':
                    case '?':
                    case '&':
                    case '=':
                    case '+':
                    case ',':
                    case '_':
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlDecode(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            int length = value.Length;
            UrlDecoder decoder = new UrlDecoder(length, encoding);
            for (int i = 0; i < length; i++)
            {
                char ch = value[i];
                if (IsUrlSafeChar(ch))
                {
                    decoder.AddByte((byte)ch);
                }
                else if ((ch == '%') && (i < (length - 2)))
                {
                    if ((value[i + 1] == 'u') && (i < (length - 5)))
                    {
                        int num3 = HexToInt(value[i + 2]);
                        int num4 = HexToInt(value[i + 3]);
                        int num5 = HexToInt(value[i + 4]);
                        int num6 = HexToInt(value[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                        {
                            if ((ch & 0xff80) == 0)
                            {
                                decoder.AddByte((byte)ch);
                            }
                            else
                            {
                                decoder.AddChar(ch);
                            }
                            continue;
                        }
                        ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num7 = HexToInt(value[i + 1]);
                    int num8 = HexToInt(value[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0))
                    {
                        byte b = (byte)((num7 << 4) | num8);
                        i += 2;
                        decoder.AddByte(b);
                        continue;
                    }
                }
            }
            return ValidateString(decoder.GetString());
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] UrlEncode(byte[] bytes, int offset, int count)
        {
            if (!ValidateByteArrayOffset(bytes, offset, count))
            {
                return null;
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (!IsUrlSafeChar(ch))
                {
                    num2++;
                }
            }
            if ((num == 0) && (num2 == 0))
            {
                if ((offset == 0) && (bytes.Length == count))
                {
                    return bytes;
                }
                byte[] dst = new byte[count];
                Buffer.BlockCopy(bytes, offset, dst, 0, count);
                return dst;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num3 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsUrlSafeChar(ch2))
                {
                    buffer[num3++] = num6;
                }
                else
                {
                    buffer[num3++] = 0x25;
                    buffer[num3++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num3++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlEncode(string str, Encoding encoding)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes(str);
            return Encoding.ASCII.GetString(UrlEncode(bytes, 0, bytes.Length));
        }

        private static string ValidateString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            int num = -1;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsSurrogate(input[i]))
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                return input;
            }
            char[] chArray = input.ToCharArray();
            for (int j = num; j < chArray.Length; j++)
            {
                char c = chArray[j];
                if (char.IsLowSurrogate(c))
                {
                    chArray[j] = (char)0xfffd;
                }
                else if (char.IsHighSurrogate(c))
                {
                    if (((j + 1) < chArray.Length) && char.IsLowSurrogate(chArray[j + 1]))
                    {
                        j++;
                    }
                    else
                    {
                        chArray[j] = (char)0xfffd;
                    }
                }
            }
            return new string(chArray);
        }

        private static bool ValidateByteArrayOffset(byte[] bytes, int offset, int count)
        {
            if (bytes == null && count == 0)
            {
                return false;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0 || offset + count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return true;
        }

        private class UrlDecoder
        {
            // Fields
            private int _bufferSize;
            private byte[] _byteBuffer;
            private char[] _charBuffer;
            private Encoding _encoding;
            private int _numBytes;
            private int _numChars;

            // Methods
            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                this._bufferSize = bufferSize;
                this._encoding = encoding;
                this._charBuffer = new char[bufferSize];
            }

            internal void AddByte(byte b)
            {
                if (this._byteBuffer == null)
                {
                    this._byteBuffer = new byte[this._bufferSize];
                }
                int index = this._numBytes;
                this._numBytes = index + 1;
                this._byteBuffer[index] = b;
            }

            internal void AddChar(char ch)
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                int index = this._numChars;
                this._numChars = index + 1;
                this._charBuffer[index] = ch;
            }

            private void FlushBytes()
            {
                if (this._numBytes > 0)
                {
                    this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                    this._numBytes = 0;
                }
            }

            internal string GetString()
            {
                if (this._numBytes > 0)
                {
                    this.FlushBytes();
                }
                if (this._numChars > 0)
                {
                    return new string(this._charBuffer, 0, this._numChars);
                }
                return string.Empty;
            }
        }
    }
}
