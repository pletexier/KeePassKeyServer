/*
 * Created by SharpDevelop.
 * User: Patrice
 * Date: 31/05/2017
 * Time: 21:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace KeyOnline
{
	partial class StatusWin
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		public System.Windows.Forms.TextBox StatusLog;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.StatusLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// StatusLog
			// 
			this.StatusLog.Location = new System.Drawing.Point(12, 12);
			this.StatusLog.Multiline = true;
			this.StatusLog.Name = "StatusLog";
			this.StatusLog.Size = new System.Drawing.Size(313, 116);
			this.StatusLog.TabIndex = 0;
			this.StatusLog.TextChanged += new System.EventHandler(this.TextBox1TextChanged);
			// 
			// StatusWin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(337, 140);
			this.Controls.Add(this.StatusLog);
			this.Name = "StatusWin";
			this.Text = "StatusWin";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
