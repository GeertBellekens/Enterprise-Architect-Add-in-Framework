namespace EAAddinFramework.Utilities
{
    partial class AddinSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddinSettingsForm));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addConfigButton = new System.Windows.Forms.Button();
            this.configListView = new BrightIdeasSoftware.ObjectListView();
            this.nameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pathColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configListView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.deleteButton);
            this.splitContainer.Panel1.Controls.Add(this.addConfigButton);
            this.splitContainer.Panel1.Controls.Add(this.configListView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Size = new System.Drawing.Size(584, 450);
            this.splitContainer.SplitterDistance = 204;
            this.splitContainer.TabIndex = 0;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Location = new System.Drawing.Point(495, 174);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // addConfigButton
            // 
            this.addConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addConfigButton.Location = new System.Drawing.Point(414, 174);
            this.addConfigButton.Name = "addConfigButton";
            this.addConfigButton.Size = new System.Drawing.Size(75, 23);
            this.addConfigButton.TabIndex = 1;
            this.addConfigButton.Text = "Add";
            this.addConfigButton.UseVisualStyleBackColor = true;
            this.addConfigButton.Click += new System.EventHandler(this.addConfigButton_Click);
            // 
            // configListView
            // 
            this.configListView.AllColumns.Add(this.nameColumn);
            this.configListView.AllColumns.Add(this.pathColumn);
            this.configListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.configListView.CellEditUseWholeCell = false;
            this.configListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.pathColumn});
            this.configListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.configListView.FullRowSelect = true;
            this.configListView.HasCollapsibleGroups = false;
            this.configListView.HideSelection = false;
            this.configListView.Location = new System.Drawing.Point(10, 10);
            this.configListView.MultiSelect = false;
            this.configListView.Name = "configListView";
            this.configListView.ShowGroups = false;
            this.configListView.Size = new System.Drawing.Size(560, 158);
            this.configListView.TabIndex = 0;
            this.configListView.UseCompatibleStateImageBehavior = false;
            this.configListView.View = System.Windows.Forms.View.Details;
            this.configListView.SelectedIndexChanged += new System.EventHandler(this.configListView_SelectedIndexChanged);
            this.configListView.DoubleClick += new System.EventHandler(this.configListView_DoubleClick);
            // 
            // nameColumn
            // 
            this.nameColumn.AspectName = "name";
            this.nameColumn.Text = "Configuration";
            this.nameColumn.Width = 150;
            // 
            // pathColumn
            // 
            this.pathColumn.AspectName = "path";
            this.pathColumn.FillsFreeSpace = true;
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 400;
            // 
            // AddinSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(584, 450);
            this.Controls.Add(this.splitContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddinSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddinSettingsForm";
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private BrightIdeasSoftware.ObjectListView configListView;
        private BrightIdeasSoftware.OLVColumn nameColumn;
        private BrightIdeasSoftware.OLVColumn pathColumn;
        private System.Windows.Forms.Button addConfigButton;
        private System.Windows.Forms.Button deleteButton;
    }
}