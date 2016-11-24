using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EAAddinFramework.WorkTracking.TFS
{
	/// <summary>
	/// Description of GetAuthorizationForm.
	/// </summary>
	public partial class GetAuthorizationForm : Form
	{
		public GetAuthorizationForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		public string userName 
		{
			get {return userNameTextBox.Text;}
			set {userNameTextBox.Text = value;}
		}
		public string passWord
		{
			get {return passwordTextBox.Text;}
			set {passwordTextBox.Text = value;}
		}
		void OkButtonClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		void CancelButtonClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
