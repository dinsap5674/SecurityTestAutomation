using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;

namespace BlobStorage.BlobService.Helper
{
    public class HashingHelper
    {
        public static string GenerateFileName(string email)
        {
            var utcDate = DateTime.UtcNow;
            var emailHash = HashEmailAddress(email);
            string filename = (utcDate.Date).ToShortDateString() + "-" + emailHash+ ".log";
            return filename;
            
        }
        public static string HashEmailAddress(string email)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(email));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString());
                }
                return builder.ToString();
            }
        }
    }
}
