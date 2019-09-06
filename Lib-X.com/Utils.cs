/*
  作者：李大熊（602365214）
*/
using System;
using System.Web;
using System.Security.Cryptography;//AES加密
using System.Text;//Base64加密
using System.Linq;
using System.IO;

/// <summary>
/// ilab-x接口通信过程中用到的各类加密、解密方法的实现，AES(256)、Base64、SHA256
/// </summary>
namespace ilabHelper
{
    public class Utils
    {
        #region 其他小工具

        /// <summary>
        /// 清除payload数据尾部补充字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim(string str)
        {
            int len = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < 16)
                {
                    len = i;
                    break;
                }
            }
            return str.Substring(0, len);
        }
        /// <summary>
        /// 将Java、Js日期转为C#日期
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertJsDateToDate(long timestamp)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long t = dt1970.Ticks + timestamp * 10000;
            return new DateTime(t);
        }

        /// <summary>
        /// 将C#日期转为Java、Js日期时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTime)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt1970.Ticks) / 10000;
        }
        /// <summary>
        /// 将C#日期转为Java、Js日期时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (DateTime.Now.Ticks - dt1970.Ticks) / 10000;
        }


        private const string dict = "0123456789ABCDEF";
        /// <summary>
        /// 生成16字符长度的随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sb.Append(dict[rand.Next(16)]);
            }
            return sb.ToString();
        }

        #endregion

        #region AES 加密与解密
        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="toEncryptArray"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AES_Encrypt(byte[] toEncryptArray, byte[] key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.None;//对应Java中的NoPadding，因为在代码中已经进行了补充

            rDel.Key = key;
            rDel.IV = key.Take(16).ToArray();

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] AES_Decrypt(byte[] cipherText, byte[] key)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold the decrypted text.
            byte[] text = null;
            // Create an AesCryptoServiceProvider object with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.None;

                aesAlg.Key = key;
                aesAlg.IV = key.Take(16).ToArray();


                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                text = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            }
            return text;
        }
        #endregion

        #region Base64加密、解密
        public static byte[] Base64_Encode(Encoding encode, byte[] source)
        {
            byte[] en_code = null;
            try
            {
                string code = Convert.ToBase64String(source);
                en_code = encode.GetBytes(code);
            }
            catch
            {
                en_code = source;
            }
            return en_code;
        }

        //UTF8编码方式，和java字节byte进行转换的方法（byte   c# 0~255  java  -128~127  ）
        public static byte[] Base64_Encode_Java(byte[] by)
        {
            sbyte[] sby = new sbyte[by.Length];
            for (int i = 0; i < by.Length; i++)
            {
                if (by[i] > 127)
                    sby[i] = (sbyte)(by[i] - 256);
                else
                    sby[i] = (sbyte)by[i];
            }
            byte[] newby = (byte[])(object)sby;
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(newby));
        }

        public static byte[] Base64_Decode(Encoding encode, string result)
        {
            //过滤特殊字符   
            string dummyData = result.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
            {
                //补全长度为4的倍数，否则下面的FromBase64String()提示长度无效
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            }
            return Convert.FromBase64String(dummyData);
        }
        #endregion

        #region SHA256加密、解密
        public static string SHA256_Encrypt(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        public static string SHA256_Hmac(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static byte[] SHA256_Hmac(string message, byte[] secret)
        {
            var encoding = new UTF8Encoding();
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(secret))
            {
                return hmacsha256.ComputeHash(messageBytes);
            }
        }
        #endregion
    }
}