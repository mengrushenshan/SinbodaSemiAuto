using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Utils
{
    public class Helper
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="input">待加密字符串</param>
        /// <param name="key">加密密钥</param>
        /// <returns>加密后字符串</returns>
        public static byte[] EncryptDES(byte[] input, byte[] key)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = key;
            provider.IV = key;
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            return mStream.ToArray();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="input">待解密字符串</param>
        /// <param name="key">解密密钥</param>
        /// <returns>解密后字符串</returns>
        public static byte[] DecryptDES(byte[] input, byte[] key)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = key;
                provider.IV = key;
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(input, 0, input.Length);
                cStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            catch (Exception e)
            {
                Common.Log.LogHelper.logSoftWare.Error("DD error", e);
                return null;
            }
        }
    }
}
