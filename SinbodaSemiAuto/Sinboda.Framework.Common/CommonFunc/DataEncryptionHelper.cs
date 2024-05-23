using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.CommonFunc
{
    public class DataEncryptionHelper
    {
        /// <summary>
        /// Des加密密钥
        /// </summary>
        private static byte[] key = { 0x0b, 0x0f, 0x06, 0x05, 0x00, 0x00, 0xcd, 0xef };

        /// <summary>
        /// Des加密初始化向量
        /// </summary>
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="input">未加密符串</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptDES(string input)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(input);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            string s = Convert.ToBase64String(mStream.ToArray());
            return s;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="output">加密符串</param>
        /// <returns>解密后字符串</returns>
        public static string DecryptDES(string output)
        {
            try
            {
                byte[] outputByteArray = Convert.FromBase64String(output);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cStream.Write(outputByteArray, 0, outputByteArray.Length);
                cStream.FlushFinalBlock();
                string s = Encoding.UTF8.GetString(mStream.ToArray());
                return s;
            }
            catch (Exception e)
            {
                Log.LogHelper.logSoftWare.Error("DD error", e);
                return null;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">未加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptMD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] palindata = Encoding.Default.GetBytes(input);//将要加密的字符串转换为字节数组
            byte[] encryptdata = md5.ComputeHash(palindata);//将字符串加密后也转换为字符数组
            return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为加密字符串
        }

        /// <summary>
        /// RSA加密数据
        /// </summary>
        /// <param name="dataToEncrypt">要加密的byte数组</param>
        /// <param name="rsaKeyInfo">RSA算法的标准参数</param>
        /// <param name="doOAEPadding">如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 System.Security.Cryptography.RSA
        ///     加密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。</param>
        /// <returns></returns>
        public static byte[] EncryptRSA(byte[] dataToEncrypt, RSAParameters rsaKeyInfo, bool doOAEPadding)
        {
            try
            {
                byte[] encryptedData;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    //导入RSA key信息，这里导入公钥信息
                    rsa.ImportParameters(rsaKeyInfo);

                    //加密传入的byte数组，并指定OAEP padding
                    //OAEP padding只可用在微软Window xp及以后的系统中
                    encryptedData = rsa.Encrypt(dataToEncrypt, doOAEPadding);
                }

                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Log.LogHelper.logSoftWare.Error("ER error", e);
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// RSA解密数据
        /// </summary>
        /// <param name="dataToDecrypt">要解密的byte数组</param>
        /// <param name="rsaKeyInfo">RSA算法的标准参数</param>
        /// <param name="doOAEPPadding">如果为 true，则使用 OAEP 填充（仅在运行 Microsoft Windows XP 或更高版本的计算机上可用）执行直接的 System.Security.Cryptography.RSA
        ///     加密；否则，如果为 false，则使用 PKCS#1 1.5 版填充。</param>
        /// <returns></returns>
        public static byte[] DecryptRSA(byte[] dataToDecrypt, RSAParameters rsaKeyInfo, bool doOAEPPadding)
        {
            try
            {
                byte[] decryptedData;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaKeyInfo);

                    decryptedData = rsa.Decrypt(dataToDecrypt, doOAEPPadding);
                }

                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Log.LogHelper.logSoftWare.Error("DR error", e);
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
