namespace EAAddinTester
{
    partial class EAAddinTesterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.menuChoice = new System.Windows.Forms.RadioButton();
        	this.locationChoice = new System.Windows.Forms.GroupBox();
        	this.diagramChoice = new System.Windows.Forms.RadioButton();
        	this.projectBrowserChoice = new System.Windows.Forms.RadioButton();
        	this.addinMenu = new System.Windows.Forms.MenuStrip();
        	this.addInsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.myTestButton = new System.Windows.Forms.Button();
        	this.locationChoice.SuspendLayout();
        	this.addinMenu.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// menuChoice
        	// 
        	this.menuChoice.AutoSize = true;
        	this.menuChoice.Location = new System.Drawing.Point(6, 19);
        	this.menuChoice.Name = "menuChoice";
        	this.menuChoice.Size = new System.Drawing.Size(52, 17);
        	this.menuChoice.TabIndex = 0;
        	this.menuChoice.Text = "Menu";
        	this.menuChoice.UseVisualStyleBackColor = true;
        	// 
        	// locationChoice
        	// 
        	this.locationChoice.Controls.Add(this.diagramChoice);
        	this.locationChoice.Controls.Add(this.projectBrowserChoice);
        	this.locationChoice.Controls.Add(this.menuChoice);
        	this.locationChoice.Location = new System.Drawing.Point(23, 21);
        	this.locationChoice.Name = "locationChoice";
        	this.locationChoice.Size = new System.Drawing.Size(143, 100);
        	this.locationChoice.TabIndex = 1;
        	this.locationChoice.TabStop = false;
        	this.locationChoice.Text = "EA Menu Location";
        	// 
        	// diagramChoice
        	// 
        	this.diagramChoice.AutoSize = true;
        	this.diagramChoice.Location = new System.Drawing.Point(6, 66);
        	this.diagramChoice.Name = "diagramChoice";
        	this.diagramChoice.Size = new System.Drawing.Size(64, 17);
        	this.diagramChoice.TabIndex = 2;
        	this.diagramChoice.Text = "Diagram";
        	this.diagramChoice.UseVisualStyleBackColor = true;
        	// 
        	// projectBrowserChoice
        	// 
        	this.projectBrowserChoice.AutoSize = true;
        	this.projectBrowserChoice.Location = new System.Drawing.Point(6, 43);
        	this.projectBrowserChoice.Name = "projectBrowserChoice";
        	this.projectBrowserChoice.Size = new System.Drawing.Size(99, 17);
        	this.projectBrowserChoice.TabIndex = 1;
        	this.projectBrowserChoice.Text = "Project Browser";
        	this.projectBrowserChoice.UseVisualStyleBackColor = true;
        	// 
        	// addinMenu
        	// 
        	this.addinMenu.Dock = System.Windows.Forms.DockStyle.None;
        	this.addinMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.addInsToolStripMenuItem});
        	this.addinMenu.Location = new System.Drawing.Point(182, 33);
        	this.addinMenu.Name = "addinMenu";
        	this.addinMenu.Size = new System.Drawing.Size(69, 24);
        	this.addinMenu.TabIndex = 2;
        	this.addinMenu.Text = "Add-Ins";
        	// 
        	// addInsToolStripMenuItem
        	// 
        	this.addInsToolStripMenuItem.Name = "addInsToolStripMenuItem";
        	this.addInsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
        	this.addInsToolStripMenuItem.Text = "Add-Ins";
        	this.addInsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.addInsToolStripMenuItem_DropDownOpening);
        	this.addInsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.addInsToolStripMenuItem_DropDownItemClicked);
        	// 
        	// myTestButton
        	// 
        	this.myTestButton.Location = new System.Drawing.Point(23, 144);
        	this.myTestButton.Name = "myTestButton";
        	this.myTestButton.Size = new System.Drawing.Size(75, 23);
        	this.myTestButton.TabIndex = 3;
        	this.myTestButton.Text = "My Test";
        	this.myTestButton.UseVisualStyleBackColor = true;
        	this.myTestButton.Click += new System.EventHandler(this.MyTestButtonClick);
        	// 
        	// EAAddinTesterForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(279, 179);
        	this.Controls.Add(this.myTestButton);
        	this.Controls.Add(this.locationChoice);
        	this.Controls.Add(this.addinMenu);
        	this.Name = "EAAddinTesterForm";
        	this.Text = "EA Add-in Tester";
        	this.locationChoice.ResumeLayout(false);
        	this.locationChoice.PerformLayout();
        	this.addinMenu.ResumeLayout(false);
        	this.addinMenu.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Button myTestButton;

        #endregion

        private System.Windows.Forms.RadioButton menuChoice;
        private System.Windows.Forms.GroupBox locationChoice;
        private System.Windows.Forms.RadioButton diagramChoice;
        private System.Windows.Forms.RadioButton projectBrowserChoice;
        private System.Windows.Forms.MenuStrip addinMenu;
        private System.Windows.Forms.ToolStripMenuItem addInsToolStripMenuItem;

    }
}

