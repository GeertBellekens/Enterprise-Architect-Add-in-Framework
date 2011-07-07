using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MyAddin
{
	[Guid("01ce6c0d-0bf0-409a-9cb6-db7d96a05a20")]
	[ComVisible(true)]
	/// <summary>
	/// Description of MyEAControl.
	/// </summary>
	public partial class MyEAControl : UserControl
	{
		public MyEAControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
		}
		public void setNameLabel(string labelName)
		{
			this.GuidLabel.Text = labelName;
		}
		

	}
}
