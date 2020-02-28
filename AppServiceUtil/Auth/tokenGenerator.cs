using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.Auth
{
    public class tokenGenerator
    {
        public static string tokenGen20(string card)
        {
            string retValue = string.Empty;
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = _converter.strtoHex(card);
                byteHash = objHashMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                string _key = _converter.getUniqueKey(8);
                byte[] _key0 = Encoding.Unicode.GetBytes(_key);
                byte[] _key1 = Encoding.Unicode.GetBytes("20100723");
                byteBuff = UTF8Encoding.UTF8.GetBytes("byEzeWqS6sjzDEnVALrLaD3XrtLZbWpc2J85e7p48CJLF4cJFg9NXSXjDnpHUT9L8zc3BkdLMPVY5nfuMtzt2Pzwz7dXzYdS8nRWMw8NgznsJmvEsUHzMcDppYC6zDyA");
                string base64str = Convert.ToBase64String(objDESCrypto.CreateEncryptor(_key0, _key1).TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                retValue = _key + _converter.strtoHex(base64str);
            }
            catch (Exception ex)
            {
                retValue = "error: " + ex.Message;
            }
            return retValue;
        }
    }
    public class _converter
    {
        public static string CharToHex(string str)
        {
            char[] charValues = str.ToCharArray();
            string hexOutput = "";
            foreach (char _eachChar in charValues)
            {
                int value = Convert.ToInt32(_eachChar);
                hexOutput += String.Format("{0:X}", value);
            }
            return hexOutput.Trim();
        }
        public static string HexToChar(string Hexstr)
        {
            byte[] vals = ToByteArray(Hexstr);
            string str = BitConverter.ToString(vals);
            string[] hexValuesSplit = str.Split('-');
            StringBuilder builder = new StringBuilder();
            foreach (String hex in hexValuesSplit)
            {
                int value = Convert.ToInt32(hex, 16);
                string stringValue = Char.ConvertFromUtf32(value);
                char charValue = (char)value;
                builder.Append(stringValue);
            }
            return builder.ToString();
        }
        public static string MD5(string org)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = ToByteArray(org);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append((hash[i].ToString("X2")).Trim());
            }
            return sb.ToString().Trim();
        }
        public static string ToUnixToHexDate(DateTime date)

        {
            string hexDate = null;
            DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            TimeSpan timeSpan = date.AddHours(-8) - unixStartTime;
            return hexDate = string.Format("{0:X}", Convert.ToInt64(timeSpan.TotalSeconds));

        }
        public static byte[] ToByteArray(String hexString)
        {
            byte[] retval = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
                retval[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return retval;
        }
        public static string DecToHex(string Dec)
        {
            int decValue = Convert.ToInt32(Dec);
            return string.Format("{0:X}", decValue);
        }
        public static string DecTo2Hex(string Dec2)
        {
            int decValue = Convert.ToInt32(Dec2);
            return string.Format("{0:X2}", decValue);
        }
        public static string DecTo4Hex(string Dec4)
        {
            int decValue = Convert.ToInt32(Dec4);
            return string.Format("{0:X4}", decValue);
        }
        public static string DecTo8Hex(string Dec8)
        {
            int decValue = Convert.ToInt32(Dec8);
            return string.Format("{0:X8}", decValue);
        }
        public static string asciitoHex(string ascii)
        {
            StringBuilder sb = new StringBuilder();
            byte[] inputBytes = Encoding.UTF8.GetBytes(ascii);
            foreach (byte b in inputBytes)
            {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }
        public string Hextoascii(string hex)
        {
            string res = String.Empty;
            for (int a = 0; a < hex.Length; a = a + 2)
            {
                string Char2Convert = hex.Substring(a, 2);
                int n = Convert.ToInt32(Char2Convert, 16);
                char c = (char)n;
                res += c.ToString();
            }
            string utf8String = res;
            string propEncodeString = string.Empty;
            byte[] utf8_Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                utf8_Bytes[i] = (byte)utf8String[i];
            }
            propEncodeString = Encoding.UTF8.GetString(utf8_Bytes, 0, utf8_Bytes.Length);
            return propEncodeString;
        }
        public static string UniToAnsi(string strTmp)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (strTmp != null)
            {
                foreach (char ch in strTmp.ToCharArray())
                {
                    if (ch > '\x00ff')
                    {
                        switch (ch)
                        {
                            case 'Ї':
                                builder.Append('\x00af');
                                break;

                            case 'ё':
                                builder.Append('\x00b8');
                                break;

                            case 'Ё':
                                builder.Append('\x00a8');
                                break;

                            case 'Є':
                                builder.Append('\x00aa');
                                break;

                            case 'є':
                                builder.Append('\x00ba');
                                break;

                            case 'ї':
                                builder.Append('\x00bf');
                                break;

                            case 'Ү':
                                builder.Append('\x00af');
                                break;

                            case 'ү':
                                builder.Append('\x00bf');
                                break;

                            case 'Ө':
                                builder.Append('\x00aa');
                                break;

                            case 'ө':
                                builder.Append('\x00ba');
                                break;

                            default:
                                if ((ch >= 'А') && (ch <= 'я'))
                                {
                                    builder.Append((char)(ch - '͐'));
                                }
                                else
                                {
                                    builder.Append((char)(ch - '\x00ff'));
                                }
                                break;
                        }
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
            }
            return builder.ToString();
        }
        public static string AnsiToUni(string strTmp)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (strTmp != null)
            {
                foreach (char ch in strTmp.ToCharArray())
                {
                    if (((((ch >= '\x00c0') && (ch <= '\x00ff')) || ((ch == '\x00a8') || (ch == '\x00aa'))) || (((ch == '\x00af') || (ch == '\x00b8')) || (ch == '\x00ba'))) || (ch == '\x00bf'))
                    {
                        switch (ch)
                        {
                            case '\x00b8':
                                builder.Append('ё');
                                break;

                            case '\x00ba':
                                builder.Append('ө');
                                break;

                            case '\x00bf':
                                builder.Append('ү');
                                break;

                            case '\x00a8':
                                builder.Append('Ё');
                                break;

                            case '\x00aa':
                                builder.Append('Ө');
                                break;

                            case '\x00af':
                                builder.Append('Ү');
                                break;

                            default:
                                builder.Append((char)(ch + '͐'));
                                break;
                        }
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
            }
            return builder.ToString();
        }
        public static string strtoHex(string strTXT)
        {
            StringBuilder sb = new StringBuilder();
            byte[] inputBytes = Encoding.UTF8.GetBytes(strTXT);
            foreach (byte b in inputBytes)
            {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }
        public static string Hextostr(string hex)
        {
            string res = String.Empty;
            for (int a = 0; a < hex.Length; a = a + 2)
            {
                string Char2Convert = hex.Substring(a, 2);
                int n = Convert.ToInt32(Char2Convert, 16);
                char c = (char)n;
                res += c.ToString();
            }
            string utf8String = res;
            string propEncodeString = string.Empty;
            byte[] utf8_Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                utf8_Bytes[i] = (byte)utf8String[i];
            }
            propEncodeString = Encoding.UTF8.GetString(utf8_Bytes, 0, utf8_Bytes.Length);
            return propEncodeString;
        }
        public static string splitPart(string input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (i % 80 == 0)
                    sb.Append(',');
                sb.Append(input[i]);
            }
            string formatted = sb.ToString();
            return formatted.TrimStart(',');
        }
        public static string minusHour(String date2) // -8 hour
        {
            String s = date2;
            string[] format = { "yyyyMMddHHmmss" };
            DateTime date;
            DateTime.TryParseExact(s,
                           format,
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out date);
            date = date.AddHours(-8);

            return date.ToString("yyyyMMddHHmmss");
        }
        public static string getUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "ABCDEF1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
