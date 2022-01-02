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
            this.settingsForm.FormClosing += new FormClosingEventHandler(this.settingFormClosing);
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
            this.settingsForm.Dock = DockStyle.Top;
            //make sure the window is not larger than the screen it it displaying on
            var screen = Screen.FromControl(this);
            if (this.Height > screen.WorkingArea.Height)
            {
                this.Height = screen.WorkingArea.Height;
            }
            //show screen
            this.settingsForm.Show();
            //select first row
            this.configListView.SelectedIndex = 0;
        }
        private void enableDisable()
        {
            this.deleteButton.Enabled = this.selectedConfig?.type == AddinConfigType.Package;
        }

        private void settingFormClosing(object sender, EventArgs e)
        {
            //reset current config to default user config
            this.settings.currentConfig = null;
            this.Close();
        }

        private AddinConfig selectedConfig { get => this.configListView.SelectedObject as AddinConfig; }

        private void configListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.settings.currentConfig = this.selectedConfig;
            this.settingsForm.refreshContents();
            enableDisable();
        }

        private void addConfigButton_Click(object sender, EventArgs e)
        {
            var newConfig = this.settings.addConfig();
            this.configListView.AddObject(newConfig);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            this.settings.deleteCurrentConfig();
            this.configListView.RemoveObject(this.configListView.SelectedObject);
            this.enableDisable();
        }

        private void configListView_DoubleClick(object sender, EventArgs e)
        {
            this.selectedConfig.openContext();
        }
    }
}
