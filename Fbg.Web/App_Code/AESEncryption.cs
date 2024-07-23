using System;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
namespace ROEDeviceUtils
{
    /// <summary>
    /// This class provide static methods for encryption and decryption.
    /// </summary>
    public class AESEncryption
    {
        /// <summary>
        /// public method
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            SecureString pwd = new SecureString();
            pwd.AppendChar('j');
            pwd.AppendChar('e');
            pwd.AppendChar('f');
            pwd.AppendChar('r');
            pwd.AppendChar('u');
            pwd.AppendChar('l');
            pwd.AppendChar('e');
            pwd.AppendChar('s');
            return Encrypt(plainText, pwd.ToString());
        }
        /// <summary>
        /// Method which does the encryption using Rijndeal algorithm
        /// </summary>
        /// <param name="plainText">Data to be encrypted</param>
        /// <param name="pwd">The string to used for making the key.The same string
        /// should be used for making the decrpt key</param>
        /// <returns>Encrypted Data</returns>
        protected static string Encrypt(string plainText, string pwd)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plainText);
            byte[] salt = Encoding.ASCII.GetBytes(pwd.Length.ToString());
            //This class uses an extension of the PBKDF1 algorithm defined in the PKCS#5 v2.0 
            //standard to derive bytes suitable for use as key material from a password. 
            //The standard is documented in IETF RRC 2898.
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(pwd, salt);
            //Creates a symmetric encryptor object. 
            ICryptoTransform encryptor = cipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            //Defines a stream that links data streams to cryptographic transformations
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            //Writes the final state and clears the buffer
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string hexstring = BitConverter.ToString(cipherBytes);
            string encryptedData = hexstring.Replace("-", "");
            return encryptedData;
        }
        /// <summary>
        /// Method which does the encryption using Rijndeal algorithm.This is for decrypting the data
        /// which has orginally being encrypted using the above method
        /// </summary>
        /// <param name="encryptedText">The encrypted data which has to be decrypted</param>
        /// <param name="pwd">The string which has been used for encrypting.The same string
        /// should be used for making the decrypt key</param>
        /// <returns>Decrypted Data</returns>
        protected static string Decrypt(string encryptedText, string pwd)
        {
            int numberChars = encryptedText.Length;
            byte[] encryptedData = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                encryptedData[i / 2] = Convert.ToByte(encryptedText.Substring(i, 2), 16);
            }
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            byte[] salt = Encoding.ASCII.GetBytes(pwd.Length.ToString());
            //Making of the key for decryption
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(pwd, salt);
            //Creates a symmetric Rijndael decryptor object.
            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(encryptedData);
            //Defines the cryptographics stream for decryption.THe stream contains decrpted data
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
            byte[] PlainText = new byte[encryptedData.Length];
            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
            memoryStream.Close();
            cryptoStream.Close();
            //Converting to string
            string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            return DecryptedData;
        }
    }
}