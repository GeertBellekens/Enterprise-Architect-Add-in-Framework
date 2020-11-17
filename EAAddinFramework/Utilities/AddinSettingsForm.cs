using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAAddinFramework.Utilities
{
    public partial class AddinSettingsForm : Form
    {
        private AddinSettings settings { get => this.settingsForm.settings; }
        private AddinSettingsFormBase settingsForm { get; set; }
        public AddinSettingsForm( AddinSettingsFormBase settingsForm)
        {
            InitializeComponent();
            this.settingsForm = settingsForm;
            this.configListView.Objects = this.settings.getAllConfigs();
            //show prototype as test
            this.Text = this.settingsForm.Text;
            this.settingsForm.TopLevel = false;
            
            this.settingsForm.FormBorderStyle = FormBorderStyle.None;
            this.splitContainer.Panel2.Controls.Add(this.settingsForm);
            //resize if needed
            if (this.settingsForm.Height > this.splitContainer.Panel2.Height)
            {
                //get the difference
                var difference = this.settingsForm.Height - this.splitContainer.Panel2.Height;
                //add to height of form
                var splitterdistance = this.splitContainer.SplitterDistance; 
                this.Height += difference;
                //set the splitter back
                this.splitContainer.SplitterDistance = splitterdistance;
            }
            //resize if needed
            if (this.settingsForm.Width > this.splitContainer.Panel2.Width)
            {
                //get the difference
                var difference = this.settingsForm.Width - this.splitContainer.Panel2.Width;
                //add to width of form
                this.Width += difference;
            }
            this.settingsForm.Dock = DockStyle.Fill;
            this.settingsForm.Show();
            //select first row
            this.configListView.SelectedIndex = 0;
        }
        private AddinConfig selectedConfig { get => this.configListView.SelectedObject as AddinConfig; }

        private void configListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.settings.currentConfig = this.selectedConfig;
            this.settingsForm.refreshContents();
        }
    }
}
