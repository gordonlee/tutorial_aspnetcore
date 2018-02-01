using System;
using System.IO;
using System.Security.Cryptography;

namespace crytography
{
    class Program
    {
        static void Main(string[] args)
        {
			string original = "server=live-openapi-testlab-cluster.cluster-cn1i0gabzxb5.ap-northeast-1.rds.amazonaws.com;database=contents;userid=admin;pwd=djemals!2;port=3306;sslmode=none;Connection Timeout=60;";

			string key = "asdkljfaskldjflkasjdfklasdjflkasdjflaskdjflkasjdflksdjfklasjldfkjasldkfjlsdddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddkfj";
			var result = cryptography.Encrypt(key, original);

			Console.WriteLine(result);

			var should_origin = cryptography.Decrypt(key, result);
			Console.WriteLine(should_origin);
			Console.ReadLine();
		}


		static void sample()
		{
			string original = "Here is some data to encrypt!";
			Console.WriteLine("Hello World!");
			byte[] encrypted = EncryptStringToBytes(original, Convert.FromBase64String(aes_key), Convert.FromBase64String(aes_iv));

			// Decrypt the bytes to a string.
			string roundtrip = DecryptStringFromBytes(encrypted, Convert.FromBase64String(aes_key), Convert.FromBase64String(aes_iv));

			//Display the original data and the decrypted data.
			Console.WriteLine("Original:   {0}", original);
			Console.WriteLine("Round Trip: {0}", roundtrip);

			Console.WriteLine("===========================");

			// Encrypt the string to an array of bytes.
			string str_encrypted = EncryptAES(original);

			// Decrypt the bytes to a string.
			string str_roundtrip = DecryptAES(str_encrypted);

			Console.WriteLine("Original: {0}", str_encrypted);
			Console.WriteLine("Round Trip: {0}", str_roundtrip);

			Console.ReadLine();
		}

		static string aes_key = "AXe8YwuIn1zxt3FPWTZFlAa14EHdPAdN9FaZ9RQWihc=";
		// static string aes_key = "enDy34hyeAscr3erg3rghSsvSEfeFVewFVeWdfv=";
		static string aes_iv = "bsxnWolsAyO7kCfWuyrnqg==";

		public static string EncryptAES(string plainText)
		{
			byte[] encrypted;

			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.Key = Convert.FromBase64String(aes_key);
				aes.IV = Convert.FromBase64String(aes_iv);
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(cs))
						{
							sw.Write(plainText);
						}

						encrypted = ms.ToArray();
					}
				}
			}

			return Convert.ToBase64String(encrypted);
		}

		public static string DecryptAES(string encryptedText)
		{
			string decrypted = null;
			byte[] cipher = Convert.FromBase64String(encryptedText);

			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.Key = Convert.FromBase64String(aes_key);
				aes.IV = Convert.FromBase64String(aes_iv);
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.PKCS7;

				ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream ms = new MemoryStream(cipher))
				{
					using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
					{
						using (StreamReader sr = new StreamReader(cs))
						{
							decrypted = sr.ReadToEnd();
						}
					}
				}
			}

			return decrypted;
		}

		static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
		{
			// Check arguments.
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");
			byte[] encrypted;
			// Create an RijndaelManaged object
			// with the specified key and IV.
			using (RijndaelManaged rijAlg = new RijndaelManaged())
			{
				rijAlg.Key = Key;
				rijAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream.
			return encrypted;

		}

		static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			// Create an RijndaelManaged object
			// with the specified key and IV.
			using (RijndaelManaged rijAlg = new RijndaelManaged())
			{
				rijAlg.Key = Key;
				rijAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;

		}
	}
}

