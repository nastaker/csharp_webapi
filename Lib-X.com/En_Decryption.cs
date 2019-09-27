/*
  作者：李大熊（602365214）
*/
using System;
using System.Web;
using System.Security.Cryptography;//AES加密
using System.Text;//Base64加密

/// <summary>
/// ilab-x接口通信过程中用到的各类加密、解密方法的实现，AES(256)、Base64、SHA256
/// </summary>
namespace RunScene.Web.ilabHelper
{
    public class En_Decryption
    {
        #region AES 加密与解密
        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="toEncryptArray"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AES_Encrypt(byte[] toEncryptArray, byte[] key, byte[] iv)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.IV = iv;
            //rDel.Mode = CipherMode.ECB;
            //rDel.Padding = PaddingMode.PKCS7;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.None;//对应Java中的NoPadding，因为在代码中已经进行了补充

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static byte[] AES_Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold the decrypted text.
            byte[] text = null;

            // Create an AesCryptoServiceProvider object with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

               // aesAlg.Padding = PaddingMode.PKCS7; 

				aesAlg.Padding = PaddingMode.None;  // 此处Padding应该是这样

                aesAlg.Mode = CipherMode.CBC;

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