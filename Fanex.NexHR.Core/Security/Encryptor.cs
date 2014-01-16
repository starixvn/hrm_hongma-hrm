// --------------------------------------------------------------------------
// <copyright file="Encryptor.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.6.14">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography;
    using System.Text;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Encryptor
    {
        public static string ComputeHash(HashProvider hashAlgorithm, string plainText, int saltSize = 8)
        {
            // Random salt
            byte[] salt = Random.RNGCrypto(saltSize);
            return ComputeHash(hashAlgorithm, plainText, salt);
        }

        /// <summary>
        /// Function to Compare String To Hash.
        /// </summary>
        /// <param name="hashAlgorithm">Hash Algorithm.</param>
        /// <param name="plainText">Plain Text.</param>
        /// <param name="encrypt">Encrypt value.</param>
        /// <param name="saltSize">Salt Size.</param>
        /// <returns>Result of compare.</returns>
        public static bool CompareStringToHash(HashProvider hashAlgorithm, string plainText, string encrypt, int saltSize = 8)
        {
            byte[] encryptBuffer = Convert.FromBase64String(encrypt);
            int encryptLength = encryptBuffer.Length;

            // Pluck the salt size off of the end of the hash
            byte[] saltBuffer = new byte[saltSize];
            Array.Copy(encryptBuffer, encryptLength - saltSize, saltBuffer, 0, saltSize);

            string hash = ComputeHash(hashAlgorithm, plainText, saltBuffer);
            return string.Equals(encrypt, hash);
        }

        /// <summary>
        /// Function to get hash provider.
        /// </summary>
        /// <param name="hashAlgorithm">Hash Algorithm.</param>
        /// <returns>HashAlgorithm object.</returns>
        private static HashAlgorithm GetHashProvider(HashProvider hashAlgorithm)
        {
            HashAlgorithm hash;
            switch (hashAlgorithm)
            {
                case HashProvider.SHA1:
                    hash = new SHA1Managed();
                    break;

                case HashProvider.SHA256:
                    hash = new SHA256Managed();
                    break;

                case HashProvider.SHA384:
                    hash = new SHA384Managed();
                    break;

                case HashProvider.SHA512:
                    hash = new SHA512Managed();
                    break;

                case HashProvider.MD5:
                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            return hash;
        }

        /// <summary>
        /// Function to compute hash.
        /// </summary>
        /// <param name="hashAlgorithm">Hash Algorithm.</param>
        /// <param name="plainText">Plain Text.</param>
        /// <param name="salt">Salt array.</param>
        /// <returns>Result of Compute.</returns>
        private static string ComputeHash(HashProvider hashAlgorithm, string plainText, byte[] salt)
        {
            // Convert plain text into a byte array
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            int dataLength = data.Length;

            // Random salt
            int saltLength = salt.Length;

            // Append the salt to the end of the plainText
            byte[] dataSalt = new byte[dataLength + saltLength];

            // Copy both the data and salt into the new array
            Array.Copy(data, dataSalt, dataLength);
            Array.Copy(salt, 0, dataSalt, dataLength, saltLength);

            // Compute hash value of our plain text with appended salt.
            byte[] encrypt = GetHashProvider(hashAlgorithm).ComputeHash(dataSalt);
            int encryptLength = encrypt.Length;

            // Append the salt to the end of the encrypt
            byte[] encryptSalt = new byte[encryptLength + saltLength];
            Array.Copy(encrypt, encryptSalt, encryptLength);
            Array.Copy(salt, 0, encryptSalt, encryptLength, saltLength);

            return Convert.ToBase64String(encryptSalt);
        }
    }
}
