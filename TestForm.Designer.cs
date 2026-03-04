namespace ALE.Controls
{
    partial class TestForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlTutorial = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.lblDesc = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDemoGroup = new System.Windows.Forms.Button();
            this.btnDemoExport = new System.Windows.Forms.Button();
            this.dataGridEx1 = new ALE.Controls.DataGridEx();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlTutorial.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlTutorial);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.splitContainer1.Panel2.Controls.Add(this.dataGridEx1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer1.Size = new System.Drawing.Size(1300, 800);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // pnlTutorial
            // 
            this.pnlTutorial.Controls.Add(this.propertyGrid1);
            this.pnlTutorial.Controls.Add(this.lblDesc);
            this.pnlTutorial.Controls.Add(this.lblTitle);
            this.pnlTutorial.Controls.Add(this.pnlButtons);
            this.pnlTutorial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTutorial.Location = new System.Drawing.Point(0, 0);
            this.pnlTutorial.Name = "pnlTutorial";
            this.pnlTutorial.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTutorial.Size = new System.Drawing.Size(350, 800);
            this.pnlTutorial.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = true;
            this.propertyGrid1.Location = new System.Drawing.Point(10, 90);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(330, 600);
            this.propertyGrid1.TabIndex = 3;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // lblDesc
            // 
            this.lblDesc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDesc.ForeColor = System.Drawing.Color.DimGray;
            this.lblDesc.Location = new System.Drawing.Point(10, 45);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(330, 45);
            this.lblDesc.TabIndex = 2;
            this.lblDesc.Text = "Change the values below to see the grid update in real-time. Only custom DataGridEx properties are shown.";
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(330, 35);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Control Properties";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnDemoGroup);
            this.pnlButtons.Controls.Add(this.btnDemoExport);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlButtons.Location = new System.Drawing.Point(10, 690);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(330, 100);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnDemoGroup
            // 
            this.btnDemoGroup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDemoGroup.Location = new System.Drawing.Point(3, 13);
            this.btnDemoGroup.Name = "btnDemoGroup";
            this.btnDemoGroup.Size = new System.Drawing.Size(300, 35);
            this.btnDemoGroup.TabIndex = 0;
            this.btnDemoGroup.Text = "Demo: Group by Department";
            this.btnDemoGroup.UseVisualStyleBackColor = true;
            // 
            // btnDemoExport
            // 
            this.btnDemoExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDemoExport.Location = new System.Drawing.Point(3, 54);
            this.btnDemoExport.Name = "btnDemoExport";
            this.btnDemoExport.Size = new System.Drawing.Size(300, 35);
            this.btnDemoExport.TabIndex = 1;
            this.btnDemoExport.Text = "Demo: Trigger CSV Export";
            this.btnDemoExport.UseVisualStyleBackColor = true;
            // 
            // dataGridEx1
            // 
            this.dataGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridEx1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dataGridEx1.Location = new System.Drawing.Point(10, 10);
            this.dataGridEx1.Margin = new System.Windows.Forms.Padding(10);
            this.dataGridEx1.Name = "dataGridEx1";
            this.dataGridEx1.RowHeight = 36;
            this.dataGridEx1.ShowContextMenu = true;
            this.dataGridEx1.ShowGlobalSearch = true;
            this.dataGridEx1.ShowThemeSelector = false;
            this.dataGridEx1.Size = new System.Drawing.Size(925, 780);
            this.dataGridEx1.TabIndex = 0;
            this.dataGridEx1.Theme = ALE.Controls.Theming.GridTheme.Clean;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 800);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DataGridEx - Interactive Tutorial & Showcase";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlTutorial.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlTutorial;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnDemoGroup;
        private System.Windows.Forms.Button btnDemoExport;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Label lblDesc;
        private Controls.DataGridEx dataGridEx1;
    }
}