using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BAL.Helper
{
   public class HelperMethods
    {
        /// <summary>
        /// Encrypt  data
        /// </summary>
        /// <param name="cipherText">Encrypted data</param>
        /// <param name="encryptionPrivateKey">Encryption key (Optional)</param>
        /// <returns></returns>
        public static string EncryptText(string plainText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = AppConfigValue.EncryptionKey;

            var tDESalg = new TripleDESCryptoServiceProvider();
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            byte[] encryptedBinary = EncryptTextToMemory(plainText, tDESalg.Key, tDESalg.IV);
            return Convert.ToBase64String(encryptedBinary).Replace("/", " ");
        }

        /// <summary>
        /// Decrypt encrypted data
        /// </summary>
        /// <param name="cipherText">Encrypted data</param>
        /// <param name="encryptionPrivateKey">Encryption key (Optional)</param>
        /// <returns></returns>
        public static string DecryptText(string cipherText, string encryptionPrivateKey = "")
        {
            if (String.IsNullOrEmpty(cipherText))
                return cipherText;

            cipherText = cipherText.Replace(" ", "/");

            if (String.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = AppConfigValue.EncryptionKey;

            var tDESalg = new TripleDESCryptoServiceProvider();
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));

            byte[] buffer = Convert.FromBase64String(cipherText);
            return DecryptTextFromMemory(buffer, tDESalg.Key, tDESalg.IV);
        }

        /// <summary>
        /// Logic to encrypt plain text
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private static byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            //Create a MemoryStream.
            MemoryStream mStream = new MemoryStream();

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new CryptoStream(mStream,
                new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv),
                CryptoStreamMode.Write);

            // Convert the passed string to a byte array.
            byte[] toEncrypt = new ASCIIEncoding().GetBytes(data);

            // Write the byte array to the crypto stream and flush it.
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            // Get an array of bytes from the 
            // MemoryStream that holds the 
            // encrypted data.
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();

            // Return the encrypted buffer.
            return ret;
        }

        /// <summary>
        /// Logic to decrypt encrypted data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private static string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            // Create a new MemoryStream using the passed 
            // array of encrypted data.
            MemoryStream msDecrypt = new MemoryStream(data);

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv),
                CryptoStreamMode.Read);

            // Create buffer to hold the decrypted data.
            byte[] fromEncrypt = new byte[data.Length];

            // Read the decrypted data out of the crypto stream
            // and place it into the temporary buffer.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            //Convert the buffer into a string and return it.
            string strReturnValue = new ASCIIEncoding().GetString(fromEncrypt).Replace("\0", "");
            return strReturnValue;
        }


        public static DateTime ConvertToDateNullable(string value, string format)
        {

            DateTime returnValue = new DateTime();
            returnValue = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            return returnValue;
        }

        public static int? ConvertToIntNullable(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            int returnValue = 0;
            int.TryParse(value, out returnValue);
            return returnValue;
        }

        public static decimal? ConvertToDecimalNullable(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            decimal returnValue = 0;
            decimal.TryParse(value, out returnValue);
            return returnValue;
        }

        public static bool? ConvertToBoolNullable(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            bool returnValue = false;
            bool.TryParse(value, out returnValue);
            return returnValue;
        }
    }
}
