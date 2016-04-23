#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Encryption.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Contracts;
using System.IO;

namespace XecMe.Common
{
    public static class Encryption
    {
        public enum SymmetricCipher { AES, RC2, Rijndael, TripleDES };

        /// <summary>
        /// Header signature AXP
        /// </summary>
        private readonly static byte[] MAGIC_BYTES = new byte[] { 65, 88, 80 };
        private const string SIGN = "AXP";

        private static CipherMode mode = CipherMode.CBC;
        private static PaddingMode padding = PaddingMode.PKCS7;
        private static SymmetricCipher cipher = SymmetricCipher.Rijndael;

        public static CipherMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }


        public static SymmetricCipher Cipher
        {
            get { return cipher; }
            set { cipher = value; }
        }


        public static PaddingMode Padding
        {
            get { return padding; }
            set { padding = value; }
        }




        public static void Encrypt(X509Certificate2 certificate, string sourceFile, string encryptedFile)
        {
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            using (SymmetricAlgorithm cipher = GetCipher())
            {
                cipher.Mode = Mode;
                cipher.Padding = Padding;
                cipher.GenerateKey();
                cipher.GenerateIV();

                using (FileStream fOut = new FileStream(encryptedFile, FileMode.Create, FileAccess.Write))
                {
                    using (CryptoStream cs = new CryptoStream(fOut, cipher.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        BinaryWriter bw = new BinaryWriter(fOut);
                        bw.Write(MAGIC_BYTES);
                        byte[] encKey = rsa.Encrypt(cipher.Key, false);
                        bw.Write((short)encKey.Length);
                        bw.Write(encKey);

                        bw.Write((short)cipher.IV.Length);
                        bw.Write(cipher.IV);

                        using (FileStream fIn = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                        {
                            ZipHelper.CopyStream(fIn, cs);
                        }
                    }
                }
            }


        }

        public static void Decrypt(X509Certificate2 certificate, string encryptedFile, string decryptedFile)
        {
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;

            using (SymmetricAlgorithm cipher = GetCipher())
            {
                cipher.Mode = Mode;
                cipher.Padding = Padding;

                using (FileStream fOut = new FileStream(decryptedFile, FileMode.Create, FileAccess.Write))
                {
                    using (FileStream fIn = new FileStream(encryptedFile, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader br = new BinaryReader(fIn);
                        byte[] sign = new byte[3];
                        br.Read(sign, 0, 3);

                        if (Encoding.UTF8.GetString(sign) != SIGN)
                        {
                            throw new FormatException("Incorrect file format");
                        }

                        byte[] encKey = new byte[br.ReadInt16()];
                        br.Read(encKey, 0, encKey.Length);
                        cipher.Key = rsa.Decrypt(encKey, false);

                        byte[] iv = new byte[br.ReadInt16()];
                        br.Read(iv, 0, iv.Length);
                        cipher.IV = iv;

                        using (CryptoStream cs = new CryptoStream(fIn, cipher.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            ZipHelper.CopyStream(cs, fOut);
                        }
                    }
                }
            }

        }

        private static SymmetricAlgorithm GetCipher()
        {
            SymmetricAlgorithm retVal = null;
            switch (Cipher)
            {
                case SymmetricCipher.AES:
                    retVal = new AesCryptoServiceProvider();
                    retVal.KeySize = 256;
                    break;
                case SymmetricCipher.Rijndael:
                    retVal = new RijndaelManaged();
                    retVal.KeySize = 256;
                    break;
                case SymmetricCipher.TripleDES:
                    retVal = new TripleDESCryptoServiceProvider();
                    retVal.KeySize = 192;
                    break;
                case SymmetricCipher.RC2:
                    retVal = new RC2CryptoServiceProvider();
                    retVal.KeySize = 128;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return retVal;
        }

    }
}
