using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAAddinFramework.Requirements.DoorsNG
{
    public partial class UserLoginForm : Form
    {
        public UserLoginForm(string defaultUserName)
        {
            InitializeComponent();
            this.userTextBox.Text = defaultUserName;
        }
        public string user { get; private set; }
        public string password { get; private set; }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.user = null;
            this.password = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.user = this.userTextBox.Text;
            this.password = this.passwordTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
