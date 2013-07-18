using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EAAddinTester
{
    public partial class EAAddinTesterForm : Form
    {
        private string location
        {
            get 
            {
                if (this.menuChoice.Checked) return "MainMenu";
                else if (this.projectBrowserChoice.Checked) return "TreeView";
                else if (this.diagramChoice.Checked) return "Diagram";
                else throw new Exception();
            }
        }

        public EAAddinTesterForm()
        {
            InitializeComponent();
            this.menuChoice.Checked = true;

        }

        private void addInsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            EAAddinTesterProgram.SetMenu(this.location, (ToolStripMenuItem)sender, string.Empty);
        }

        public void addInsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            EAAddinTesterProgram.clickMenu(this.location,e.ClickedItem.OwnerItem.Text,e.ClickedItem.Text);
        }
        
        void MyTestButtonClick(object sender, EventArgs e)
        {
        	EAAddinTesterProgram.myTest();
        }
    }
}
