using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PartsReserver.Models;

namespace PartsReserver
{
	internal class LicHelper
	{
		internal const string Alg = "SHA256";
		private static readonly string SignKey;
		private static readonly string EncryptKey;

		static LicHelper()
		{
			SignKey = GetKey("Sign");
			EncryptKey = GetKey("Encrypt");
		}

		public static License GetLicense()
		{
			try
			{
				var licenseFile = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\license.lic");
				var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(
					Encoding.ASCII.GetString(licenseFile));

				var license = Convert.FromBase64String(dict["license"]);
				var signature = Convert.FromBase64String(dict["signature"]);
				
				ValidateSignature(GetHash(license), signature);

				return Decrypt(license);
			}
			catch (LicenseValidationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				Logger.Write("Invalid license", exception);
				throw new LicenseValidationException("Invalid license", exception);
			}
		}

		public static void ValidateLicense()
		{
			try
			{
				var unpackedLicense = GetLicense();
				if (DateTime.Now > unpackedLicense.Validity)
				{
					throw new LicenseValidationException("License has expired");
				}
			}
			catch (LicenseValidationException e)
			{
				Logger.Write("LicenseValidationException", e);
				throw;
			}
			catch (Exception exception)
			{
				Logger.Write("Invalid license", exception);
				throw new LicenseValidationException("Invalid license", exception);
			}
		}

		public static Thread StartValidation(Func<bool> stopped)
		{
			var validator = new Thread(() =>
			{
				var rnd = new Random();
				Stopwatch stopwatch = Stopwatch.StartNew();
				var next = 0;
				while (!stopped())
				{
					if (stopwatch.ElapsedMilliseconds > next)
					{
						ValidateLicense();
						next = rnd.Next(15 * 1000);
						stopwatch = Stopwatch.StartNew();
					}

					Thread.Sleep(200);
				}
			});
			validator.Start();
			return validator;
		}

		private static string GetKey(string key)
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"PartsReserver.Resources.{key}.xml"))
			{
				if (stream == null)
				{
					throw new LicenseValidationException($"Failed to load key: {key}");
				}

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private static License Decrypt(byte[] encrypted)
		{
			using (var csp = new RSACryptoServiceProvider())
			{
				csp.FromXmlString(EncryptKey);

				var decrypt = csp.Decrypt(encrypted, true);
				return JsonConvert.DeserializeObject<License>(Encoding.ASCII.GetString(decrypt));
			}
		}

		private static byte[] GetHash(byte[] data)
		{
			using (var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(Alg))
			{
				return algorithm.ComputeHash(data);
			}
		}

		private static void ValidateSignature(byte[] hash, byte[] signature)
		{
			var csp = new RSACryptoServiceProvider();
			csp.FromXmlString(SignKey);

			if (!csp.VerifyHash(hash, CryptoConfig.MapNameToOID(Alg), signature))
			{
				throw new LicenseValidationException("Invalid signature");
			}
		}
	}

	public class LicenseValidationException : Exception
	{
		public LicenseValidationException(string msg) : base(msg) { }

		public LicenseValidationException(string msg, Exception innerException) : base(msg, innerException) { }
	}
}
