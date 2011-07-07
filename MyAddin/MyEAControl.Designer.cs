/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 30/06/2011
 * Time: 4:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace MyAddin
{
	partial class MyEAControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.GuidLabel = new System.Windows.Forms.Label();
			this.labelForGUID = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// GuidLabel
			// 
			this.GuidLabel.Location = new System.Drawing.Point(22, 61);
			this.GuidLabel.Name = "GuidLabel";
			this.GuidLabel.Size = new System.Drawing.Size(244, 23);
			this.GuidLabel.TabIndex = 0;
			this.GuidLabel.Text = "GUID";
			// 
			// labelForGUID
			// 
			this.labelForGUID.Location = new System.Drawing.Point(22, 38);
			this.labelForGUID.Name = "labelForGUID";
			this.labelForGUID.Size = new System.Drawing.Size(145, 23);
			this.labelForGUID.TabIndex = 1;
			this.labelForGUID.Text = "GUID of selected element";
			// 
			// MyEAControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelForGUID);
			this.Controls.Add(this.GuidLabel);
			this.Name = "MyEAControl";
			this.Size = new System.Drawing.Size(269, 140);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label labelForGUID;
		private System.Windows.Forms.Label GuidLabel;
	}
}
