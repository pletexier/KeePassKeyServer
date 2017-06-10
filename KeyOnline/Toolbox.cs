/*
 * Created by SharpDevelop.
 * User: Patrice
 * Date: 10/06/2017
 * Time: 10:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Security.Cryptography;

namespace KeyOnline
{
	public class SecureString
	{
		/// <summary>
		/// Entropy byte array for data encryption
		/// </summary>
		private byte[] EncryptEntropy = { 1, 2, 3, 5, 7, 11, 13, 17 };
		private string EncryptedValue = string.Empty;
		
		/// <summary>
		/// Function EncryptStringLocalUser
		/// Encrypts a string using local user information
		/// </summary>
		
		public string Encrypted
		{
			get { return this.EncryptedValue; }
			set { this.EncryptedValue = value; }
		}
		
		public string Decrypted
		{
			get { return this.Decrypt(this.EncryptedValue); }
			set { this.EncryptedValue = this.Encrypt(value); }
		}
		
		private string Encrypt(string inputstr)
		{
				// Convert password to UTF8 byte array;
				byte[] PasswordDecrypted = Encoding.UTF8.GetBytes(inputstr);
				// Encrypt Password using local user informations
				byte[] PasswordEncrypted = ProtectedData.Protect(PasswordDecrypted,EncryptEntropy,DataProtectionScope.CurrentUser);
				// Clear decrypted password array for security
				Array.Clear(PasswordDecrypted,0,PasswordDecrypted.Length);
				// Convert Encrypted Password to Base64 for easy storage
				string PasswordFinal = Convert.ToBase64String(PasswordEncrypted);
				// Clear Encrypted Password array for security
				Array.Clear(PasswordEncrypted,0,PasswordEncrypted.Length);
				
				return PasswordFinal;
		}

		/// <summary>
		/// Function DecryptStringLocalUser
		/// Decrypts a string using local user information
		/// </summary>
		private string Decrypt(string inputstr)
		{
				// Convert Password to byte array
				byte[] PasswordEncrypted = Convert.FromBase64String(inputstr);
				// Decrypt Password using local user informations
				byte[] PasswordDecrypted = ProtectedData.Unprotect(PasswordEncrypted,EncryptEntropy,DataProtectionScope.CurrentUser);
				// Clear PasswordEncrypted for security
				Array.Clear(PasswordEncrypted,0,PasswordEncrypted.Length);
				// Convert password to UTF8 string
				string PasswordFinal = Encoding.UTF8.GetString(PasswordDecrypted);
				// Clear PasswordDecrypted for security
				Array.Clear(PasswordDecrypted,0,PasswordDecrypted.Length);
				
				return PasswordFinal;
		}

	}
}
