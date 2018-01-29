using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace aes_cryptography
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var encrypted = Encrypt("thisiskey", "hello world");
            Console.WriteLine(encrypted);

            Console.WriteLine(Decrypt("thisiskey",encrypted));
        }


		static private string Encrypt(string key, string clearText)
		{
            string EncryptionKey = key;
			byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, 
                                                                new byte[] { 0x49, 0x76, 0x65, 0x6e, 0x20, 0x76, 0x61, 0x64, 0x4d , 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
                    clearText = "crypto::" + Convert.ToBase64String(ms.ToArray());
				}
			}
			return clearText;
		}


		static private string Decrypt(string key, string cipherText)
		{
            if (cipherText.StartsWith("crypto::", StringComparison.Ordinal))
            {
                cipherText = cipherText.Substring("crypto::".Length);
            }
            else
            {
                return "";
            }

            string EncryptionKey = key;
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, 
                                                                new byte[] { 0x49, 0x76, 0x65, 0x6e, 0x20, 0x76, 0x61, 0x64, 0x4d, 0x65, 0x64, 0x65, 0x76 });
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return cipherText;
		}

    }
}
