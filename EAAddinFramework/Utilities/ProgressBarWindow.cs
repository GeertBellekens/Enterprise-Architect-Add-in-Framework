using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAAddinFramework.Utilities
{
    public partial class ProgressBarWindow : Form
    {
        private string command;
        private string arguments;
        public ProgressBarWindow()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }
        public string execute(string command, string arguments, string title, string label)
        {
            this.Text = title;
            this.label.Text = label;
            this.command = command;
            this.arguments = arguments;
            this.ShowDialog();
            return this.outputTextBox.Text;
        }
        private void runCommand(string command, string arguments)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = command;
            processStartInfo.Arguments = arguments;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            var process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            this.outputTextBox.Text = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private async void ProgressBarWindow_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.command))
            {
                await Task.Run(() => { this.runCommand(this.command, this.arguments); });
                this.label.Text = "Finished";
                this.progressBar.Style = ProgressBarStyle.Continuous;
                this.progressBar.MarqueeAnimationSpeed = 0;
            }
        }
    }
}
