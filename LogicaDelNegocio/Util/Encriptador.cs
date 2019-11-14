using System;
using System.Security.Cryptography;
using System.Text;

namespace LogicaDelNegocio.Util
{
    public class Encriptador
    {
        /// <summary>
        /// Calcula el Hash de una cadena con el algoritmo Sha256
        /// </summary>
        /// <param name="rawData">String</param>
        /// <returns>String</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            if(rawData != null || rawData == String.Empty)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            return String.Empty;
        }
    }
}
