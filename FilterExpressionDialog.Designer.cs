namespace ALE.Controls.Filtering
{
    partial class FilterExpressionDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblHeader = new Label();
            splitContainer1 = new SplitContainer();
            leftPanel = new Panel();
            txtSearch = new TextBox();
            lstFields = new ListBox();
            rightPanel = new Panel();
            scrollPanel = new Panel();
            bottomPanel = new Panel();
            previewPanel = new Panel();
            _lblPreview = new Label();
            btnPanel = new FlowLayoutPanel();
            btnCancel = new Button();
            btnApply = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            previewPanel.SuspendLayout();
            btnPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblHeader
            // 
            lblHeader.Dock = DockStyle.Top;
            lblHeader.Font = new Font("Segoe UI", 11F);
            lblHeader.Location = new Point(0, 0);
            lblHeader.Margin = new Padding(4, 0, 4, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Padding = new Padding(20, 19, 10, 12);
            lblHeader.Size = new Size(1125, 69);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "Define filter conditions below. Double-click a field to add it as a condition.";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 69);
            splitContainer1.Margin = new Padding(4, 5, 4, 5);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(leftPanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rightPanel);
            splitContainer1.Size = new Size(1125, 713);
            splitContainer1.SplitterDistance = 275;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 1;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(lstFields);
            leftPanel.Controls.Add(txtSearch);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Margin = new Padding(4, 5, 4, 5);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(15, 12, 15, 12);
            leftPanel.Size = new Size(275, 713);
            leftPanel.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Top;            
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.Location = new Point(15, 12);
            txtSearch.Margin = new Padding(4, 5, 4, 5);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search fields...";
            txtSearch.Size = new Size(245, 31);
            txtSearch.TabIndex = 1;
            // 
            // lstFields
            // 
            lstFields.BorderStyle = BorderStyle.FixedSingle;
            lstFields.Dock = DockStyle.Fill;
            lstFields.Font = new Font("Segoe UI", 9F);
            lstFields.FormattingEnabled = true;
            lstFields.IntegralHeight = false;
            lstFields.ItemHeight = 25;
            lstFields.Location = new Point(15, 43);
            lstFields.Margin = new Padding(4, 5, 4, 5);
            lstFields.Name = "lstFields";
            lstFields.Size = new Size(245, 658);
            lstFields.TabIndex = 0;
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(scrollPanel);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(0, 0);
            rightPanel.Margin = new Padding(4, 5, 4, 5);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10, 12, 10, 12);
            rightPanel.Size = new Size(849, 713);
            rightPanel.TabIndex = 0;
            // 
            // scrollPanel
            // 
            scrollPanel.AutoScroll = true;
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.Location = new Point(10, 12);
            scrollPanel.Margin = new Padding(4, 5, 4, 5);
            scrollPanel.Name = "scrollPanel";
            scrollPanel.Padding = new Padding(10, 12, 10, 12);
            scrollPanel.Size = new Size(829, 689);
            scrollPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.Controls.Add(previewPanel);
            bottomPanel.Controls.Add(btnPanel);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(0, 782);
            bottomPanel.Margin = new Padding(4, 5, 4, 5);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1125, 156);
            bottomPanel.TabIndex = 2;
            // 
            // previewPanel
            // 
            previewPanel.Controls.Add(_lblPreview);
            previewPanel.Dock = DockStyle.Top;
            previewPanel.Location = new Point(0, 0);
            previewPanel.Margin = new Padding(4, 5, 4, 5);
            previewPanel.Name = "previewPanel";
            previewPanel.Padding = new Padding(20, 16, 20, 16);
            previewPanel.Size = new Size(1125, 81);
            previewPanel.TabIndex = 1;
            // 
            // _lblPreview
            // 
            _lblPreview.Dock = DockStyle.Fill;
            _lblPreview.Font = new Font("Consolas", 9.5F);
            _lblPreview.Location = new Point(20, 16);
            _lblPreview.Margin = new Padding(4, 0, 4, 0);
            _lblPreview.Name = "_lblPreview";
            _lblPreview.Size = new Size(1085, 49);
            _lblPreview.TabIndex = 0;
            _lblPreview.Text = "No filter defined";
            // 
            // btnPanel
            // 
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnApply);
            btnPanel.Dock = DockStyle.Bottom;
            btnPanel.FlowDirection = FlowDirection.RightToLeft;
            btnPanel.Location = new Point(0, 81);
            btnPanel.Margin = new Padding(4, 5, 4, 5);
            btnPanel.Name = "btnPanel";
            btnPanel.Padding = new Padding(15, 12, 15, 12);
            btnPanel.Size = new Size(1125, 75);
            btnPanel.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.Location = new Point(976, 12);
            btnCancel.Margin = new Padding(4, 0, 0, 0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(119, 50);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnApply
            // 
            btnApply.Font = new Font("Segoe UI", 9F);
            btnApply.Location = new Point(847, 12);
            btnApply.Margin = new Padding(10, 0, 0, 0);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(125, 50);
            btnApply.TabIndex = 0;
            btnApply.Text = "Apply";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += btnApply_Click;
            // 
            // FilterExpressionDialog
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1125, 938);
            Controls.Add(splitContainer1);
            Controls.Add(lblHeader);
            Controls.Add(bottomPanel);
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(870, 672);
            Name = "FilterExpressionDialog";
            StartPosition = FormStartPosition.CenterParent;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            bottomPanel.ResumeLayout(false);
            previewPanel.ResumeLayout(false);
            btnPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListBox lstFields;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Panel scrollPanel;
        protected ALE.Controls.Filtering.FilterGroupPanel _rootPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Panel previewPanel;
        protected System.Windows.Forms.Label _lblPreview;
        private System.Windows.Forms.FlowLayoutPanel btnPanel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}