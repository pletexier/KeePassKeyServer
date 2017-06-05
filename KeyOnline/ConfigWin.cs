/*
 * Created by SharpDevelop.
 * User: Patrice
 * Date: 04/06/2017
 * Time: 21:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

using KeePass;
using KeePass.Plugins;

namespace KeyOnline.Forms
{
	/// <summary>
	/// Description of ConfigWin.
	/// </summary>
	public partial class ConfigWin : Form
	{
		private IPluginHost m_host = null;
		
		// Constantes Fichier Config
		private string configBase;
		private string urlField;
		private string loginField;
		private string passwordField;
		
		public ConfigWin(IPluginHost host)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			m_host = host;

			// Constantes Fichier Config
			configBase = "KeyServer.";
			urlField = configBase + "url";
			loginField = configBase + "login";
			passwordField = configBase + "Password";
		
			// Lecture des informations de configuration
			this.textBoxURL.Text = m_host.CustomConfig.GetString(urlField);
			this.textBoxLogin.Text = m_host.CustomConfig.GetString(loginField);
			this.textBoxPassword.Text = m_host.CustomConfig.GetString(passwordField);
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{	// Bouton OK
			
			// Ecriture des informations de configuration
			m_host.CustomConfig.SetString(urlField,this.textBoxURL.Text);
			m_host.CustomConfig.SetString(loginField,this.textBoxLogin.Text);
			m_host.CustomConfig.SetString(passwordField,this.textBoxPassword.Text);
			
			// Fermeture de la fenêtre
			this.Close();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
	
		}
	}
}
