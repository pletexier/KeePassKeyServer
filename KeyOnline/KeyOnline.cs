/*
 * Created by SharpDevelop.
 * User: Patrice
 * Date: 28/05/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
//using System.Management;
using Security;

using KeePass.App;
using KeePass.Plugins;
using KeePassLib.Keys;

using KeyOnline.Forms;

namespace KeyOnline
{
	public sealed class KeyOnlineExt : Plugin
	{
		private IPluginHost m_host = null;
		private KeyOnline m_provider = null;

		// Objects for menu items
		private ToolStripSeparator m_tsSeparator = null;
		private ToolStripMenuItem m_tsmiMenuItem = null;
		
		public override bool Initialize(IPluginHost host)
		{
			m_provider = new KeyOnline(host);
			m_host = host;
			
			// Add Key Provider
			m_host.KeyProviderPool.Add(m_provider);
			
			// Add "KeyServer Configuration" to the Tools menu
			AddConfigMenu();
			
			return true;
		}

		public override void Terminate()
		{
			// Remove all of our menu items
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;
			m_tsmiMenuItem.Click -= this.OpenConfigForm;
			tsMenu.Remove(m_tsmiMenuItem);
			tsMenu.Remove(m_tsSeparator);

			// Remove KeyProvider			
			m_host.KeyProviderPool.Remove(m_provider);
		}

		// Open KeyServer Configuration Form
		private void OpenConfigForm(object sender, EventArgs e)
		{
			ConfigWin ConfigForm = new ConfigWin(m_host);
			ConfigForm.ShowDialog();
		}

		// Add "KeyServer Configuration" to the Tools menu
		private void AddConfigMenu()
		{
			// Get a reference to the 'Tools' menu item container
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;
		
			// Add a separator at the bottom
			m_tsSeparator = new ToolStripSeparator();
			tsMenu.Add(m_tsSeparator);
		
			// Add menu item
			m_tsmiMenuItem = new ToolStripMenuItem();
			m_tsmiMenuItem.Text = "Configure KeyServer";
			m_tsmiMenuItem.Click += this.OpenConfigForm;
			tsMenu.Add(m_tsmiMenuItem);
		}
	}

	public sealed class KeyOnline : KeyProvider
	{
		private IPluginHost m_host = null;
		
		// Constants
		// Name of custom items in Keepass.config.xml
		const string configBase = "KeyServer.";
		const string urlField = configBase + "url";
		const string loginField = configBase + "login";
		const string passwordField = configBase + "Password";

		// HarwareId generated during plugin initialization
		private string HardwareId = string.Empty;

		/// <summary>
		/// Init: Calculate hardwareid.
		/// </summary>
		public KeyOnline(IPluginHost host)
		{
			m_host = host;
			FingerPrint clFingerPrint = new FingerPrint();
			HardwareId = clFingerPrint.Value();
		}
		
		public override string Name
		{
			get { return "KeyOnline Key Provider"; }
		}

		public override byte[] GetKey(KeyProviderQueryContext ctx)
		{	
			//Apply Policy restrictions.
			PolicyApply();
			
			SecureString secPassword = new SecureString();

			// Reads config values for Keepass.config.xml
			string ksURL = m_host.CustomConfig.GetString(urlField);
			string ksLogin = m_host.CustomConfig.GetString(loginField);
			string ksPassword = m_host.CustomConfig.GetString(passwordField);
			
			// Open config window if a parameter is missing
			if ((ksURL == null) || (ksLogin == null) || (ksPassword == null))
			{
				ConfigWin ConfigForm = new ConfigWin(m_host);
				ConfigForm.ShowDialog();
				
				ksURL = m_host.CustomConfig.GetString(urlField);
				ksLogin = m_host.CustomConfig.GetString(loginField);
				ksPassword = m_host.CustomConfig.GetString(passwordField);
			}

			// Open Debug Window			
			StatusWin StatusForm = new StatusWin();
			StatusForm.Show();
			
			// Create Webservice object
			  StatusForm.StatusLog.Text = "Invoking Web Service...";
			KeyServerWS.Soap ksws = new KeyServerWS.Soap();
			// Use URL from Keepass.config.xml
			ksws.Url = ksURL;				
			  StatusForm.StatusLog.AppendText("Done\r\n");
			
			// Retrieve database filename
			string dbFilename = Path.GetFileName(ctx.DatabasePath);
			  StatusForm.StatusLog.AppendText("Querying key with parameters :\r\n");
			  StatusForm.StatusLog.AppendText(String.Format(" - FileName : {0}\r\n",dbFilename));
			  StatusForm.StatusLog.AppendText(String.Format(" - UserName : {0}\r\n",ksLogin));
			  StatusForm.StatusLog.AppendText(String.Format(" - HadwareId : {0}\r\n",HardwareId));

			string dbKey = null;
			try {
				secPassword.Encrypted = ksPassword;
				NetworkCredential ksCreds = new NetworkCredential();
				ksCreds.UserName = ksLogin;
				ksCreds.Password = secPassword.Decrypted;
				ksws.Credentials = ksCreds;
				dbKey = ksws.GetKey(dbFilename,ksLogin,HardwareId);
				  StatusForm.StatusLog.AppendText("Done\r\n");
				  StatusForm.StatusLog.AppendText(String.Format("Key Found : {0}\r\n",dbKey));
			} catch (Exception) {
				MessageBox.Show("Connection Error...\r\nCheck configuration.\r\n(Tools / Configure KeyServer)");
				StatusForm.Close();
				return null;
			}
			
			if (dbKey == String.Empty)
			{
				MessageBox.Show("Error : Access denied or database not found");
				StatusForm.Close();
				return null;
			}
			
			return Encoding.ASCII.GetBytes(dbKey);
		}

		// Apply restricted Plugin Policy
		private void PolicyApply()
		{
			KeePass.App.AppPolicy.Current.ChangeMasterKey = false;
			KeePass.App.AppPolicy.Current.ChangeMasterKeyNoKey = false;
			KeePass.App.AppPolicy.Current.Export = false;
			KeePass.App.AppPolicy.Current.ExportNoKey = false;
			KeePass.App.AppPolicy.Current.Print = false;
			KeePass.App.AppPolicy.Current.PrintNoKey = false;
			KeePass.App.AppPolicy.Current.UnhidePasswords = false;
			KeePass.App.AppPolicy.Current.EditTriggers = false;
			KeePass.App.AppPolicy.Current.CopyWholeEntries = false;

			EventHandler<KeePass.UI.GwmWindowEventArgs> ehEditEntryOpen = delegate(object sender, KeePass.UI.GwmWindowEventArgs e)
			{
				if (e.Form.Name=="PwEntryForm")	{ KeePass.App.AppPolicy.Current.UnhidePasswords = true; }
			};
			KeePass.UI.GlobalWindowManager.WindowAdded += ehEditEntryOpen;
			EventHandler<KeePass.UI.GwmWindowEventArgs> ehEditEntryClose = delegate(object sender, KeePass.UI.GwmWindowEventArgs e)
			{
				if (e.Form.Name=="PwEntryForm")	{ KeePass.App.AppPolicy.Current.UnhidePasswords = false; }
			};
			KeePass.UI.GlobalWindowManager.WindowRemoved += ehEditEntryClose;
		}
		
	}
}

