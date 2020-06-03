using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppServiceUtil.Utils
{
    public class convertors
    {
        public static string getUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "1234567890".ToCharArray();
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
        public static string encodeBase64(string orginalTxt)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(orginalTxt);
            return Convert.ToBase64String(byt);
        }
        public static string decodeBase64(string base64Txt)
        {
            byte[] b = Convert.FromBase64String(base64Txt);
            return Encoding.UTF8.GetString(b);
        }
        public static string generateToken(string phone)
        {
            string retValue = string.Empty;
            try
            {
                string _key = phone;
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = convertors.strtoHex(rndStr(8));
                byteHash = objHashMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byte[] _key0 = Encoding.Unicode.GetBytes(rndStr(8));
                byte[] _key1 = Encoding.Unicode.GetBytes(_key);
                byteBuff = UTF8Encoding.UTF8.GetBytes("token");
                string base64str = Convert.ToBase64String(objDESCrypto.CreateEncryptor(_key0, _key1).TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                retValue = convertors.strtoHex(base64str);
            }
            catch (Exception ex)
            {
                retValue = "error: " + ex.Message;
            }
            return retValue;
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

        public static string rndStr(int maxSize)
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
