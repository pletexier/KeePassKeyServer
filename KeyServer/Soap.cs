/*
 * Created by SharpDevelop.
 * User: Patrice
 * Date: 31/05/2017
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace KeyServer
{
	[WebService]
	public class Soap : System.Web.Services.WebService
	{
		// Path of XML files
		const string XmlPath = @"C:\@Almeria\KeePass\KeyOnline\KeyServer\";

		/// <summary>
		/// Logs into the web service
		/// </summary>
		/// <param name="userName">The User Name to login in as</param>
		/// <param name="password">User's password</param>
		/// <returns>True on successful login.</returns>
		[WebMethod(EnableSession=true)]
		public bool Login(string userName, string password)
		{
			//NOTE: There are better ways of doing authentication. This is just illustrates Session usage.
			UserName = userName;
			return true;
		}
		
		/// <summary>
		/// Logs out of the Session.
		/// </summary>
		[WebMethod(EnableSession=true)]
		public void Logout()
		{    
			Context.Session.Abandon();
		}

		/// <summary>
		/// Main Method
		/// Check permissions and return key.
		/// </summary>
		[WebMethod(EnableSession=true)]
		public string GetKey(string db, string username, string hardwareid)
		{
	   	  // Log request
	   	  DataTable RequestLog = CreateLogTable();
	   	  DataRow RequestLogRow;
		  
	   	  RequestLogRow = RequestLog.NewRow();
	   	  RequestLogRow["db"] = db;
	   	  RequestLogRow["user"] = UserName;
	   	  RequestLogRow["hardwareid"] = hardwareid;
	   	  RequestLog.Rows.Add(RequestLogRow);
	   	  
	   	  string RequestLogPath = XmlPath + "Requests.xml";
	   	  FileStream RequestLogStream = File.Create(RequestLogPath);
	   	  RequestLog.WriteXml(RequestLogStream,XmlWriteMode.WriteSchema);
	   	  RequestLogStream.Close();

		  // Check permission in permission.xml
		  if (CheckPermission(db,username,hardwareid))
		  {
		  	return GetDbKey(db); 
		  }
		  else
		  {
		  	return string.Empty;
		  }
		}
		
		/// <summary>
		/// UserName of the logged in user.
		/// </summary>
		private string UserName {
			get {return (string)Context.Session["User"];}
			set {Context.Session["User"] = value;}
		}
		
		/// <summary>
		/// Returns the key of the specified db
		/// </summary>
		private string GetDbKey(string db)
		{
			DataTable DbKeys = new DataTable();
			string Key = string.Empty;
			DbKeys.ReadXml(XmlPath + "Keys.xml");
			
			DataRow[] foundRows = DbKeys.Select("db = '" + db + "'");
			if ( foundRows.Length > 0 )
			{
				Key = (string)foundRows[0][1];
			}
			return Key;
		}

		/// <summary>
		/// Checks if the specified user and hardwareid is allowed to open the specified db.
		/// Returns true when a result is found.
		/// </summary>
		private Boolean CheckPermission(string db, string username, string hardwareid)
		{
			DataTable Permissions = new DataTable();
			Permissions.ReadXml(XmlPath + "Permissions.xml");
			
			DataRow[] foundRows;
			foundRows = Permissions.Select("db = '" + db + "' AND user='" + username + "' AND hardwareid = '" + hardwareid + "'");
			
			return (foundRows.Length > 0);
		}
				
		/// <summary>
		/// Returns a DataTable to log requests
		/// Columns :
		///   - db = Filename (ex : mypasswords.kdbx)
		///   - user = username
		///	  - hardwareid = 16 byte Unique Identification code of a computer
    	///     Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9 
		/// </summary>
		private DataTable CreateLogTable()
		{
	   	  DataTable RequestLog = new DataTable("RequestLog");
	   	  DataColumn column;
	   	  
	   	  column = new DataColumn();
	   	  column.DataType = Type.GetType("System.String");
		  column.ColumnName = "db";
		  RequestLog.Columns.Add(column);

		  column = new DataColumn();
	   	  column.DataType = Type.GetType("System.String");
		  column.ColumnName = "user";
		  RequestLog.Columns.Add(column);
		  
		  column = new DataColumn();
	   	  column.DataType = Type.GetType("System.String");
		  column.ColumnName = "hardwareid";
		  RequestLog.Columns.Add(column);
		  
		  return RequestLog;
		}
	}
}
