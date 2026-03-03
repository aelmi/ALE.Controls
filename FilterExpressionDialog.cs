using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ALE.Controls.Theming;

namespace ALE.Controls.Filtering
{
    public partial class FilterExpressionDialog : Form
    {
        private readonly List<(string Property, string Header)> _allFields;
        private readonly Type _dataSourceType;
        private readonly GridTheme _theme;
        private ThemeColors _colors;

        public FilterGroup ResultFilter { get; private set; }

        public FilterExpressionDialog(
            List<(string Property, string Header)> allFields,
            Type dataSourceType,
            FilterGroup existingFilter = null,
            GridTheme theme = GridTheme.Light)
        {
            _allFields = allFields ?? new List<(string, string)>();
            _dataSourceType = dataSourceType;
            _theme = theme;
            _colors = ThemePalette.GetPalette(theme);

            InitializeComponent();

            Text = "Filter Builder";
            Size = new Size(900, 600);
            MinimumSize = new Size(700, 450);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9f);
            FormBorderStyle = FormBorderStyle.Sizable;
            ShowInTaskbar = false;
            ShowIcon = false;

            foreach (var field in _allFields.OrderBy(f => f.Header))
            {
                lstFields.Items.Add(new FieldListItem(field.Property, field.Header));
            }

            // Pass Type to panel
            _rootPanel = new FilterGroupPanel(_allFields, _dataSourceType, existingFilter, isRoot: true, _theme);
            scrollPanel.Controls.Add(_rootPanel);

            txtSearch.TextChanged += (_, _) => FilterFieldList(lstFields, txtSearch.Text);

            lstFields.DoubleClick += (_, _) =>
            {
                if (lstFields.SelectedItem is FieldListItem item)
                {
                    _rootPanel.AddConditionForProperty(item.Property);
                }
            };

            _rootPanel.FilterChanged += (_, _) => UpdatePreview();

            Load += (s, e) =>
            {
                if (splitContainer1.Width > 400)
                {
                    splitContainer1.SplitterDistance = 220;
                }
            };

            ApplyTheme();

            if (existingFilter != null && !existingFilter.IsEmpty)
            {
                _rootPanel.RebuildFromGroup(existingFilter);
            }

            UpdatePreview();
        }

        private void ApplyTheme()
        {
            var p = _colors;

            BackColor = p.ToolbarBackground;
            ForeColor = p.TextPrimary;

            lblHeader.BackColor = p.ToolbarBackground;
            lblHeader.ForeColor = p.TextPrimary;

            leftPanel.BackColor = p.GridBackground;
            txtSearch.BackColor = p.RowEven;
            txtSearch.ForeColor = p.TextPrimary;
            lstFields.BackColor = p.RowEven;
            lstFields.ForeColor = p.TextPrimary;

            rightPanel.BackColor = p.GridBackground;
            scrollPanel.BackColor = p.GridBackground;

            previewPanel.BackColor = p.HeaderBackground;
            _lblPreview.BackColor = p.HeaderBackground;
            _lblPreview.ForeColor = p.HeaderText;

            // Style Apply and Cancel buttons
            StyleButton(btnApply, p.HeaderBackground, p.HeaderText, isPrimary: true);
            StyleButton(btnCancel, p.RowOdd, p.TextPrimary);
        }

        private void StyleButton(Button btn, Color backColor, Color foreColor, bool isPrimary = false)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = isPrimary ? 0 : 1;
            btn.FlatAppearance.BorderColor = isPrimary ? backColor : Color.FromArgb(200, 200, 200);
            btn.Cursor = Cursors.Hand;
            btn.Padding = new Padding(12, 0, 12, 0);
        }

        private void FilterFieldList(ListBox listBox, string search)
        {
            listBox.BeginUpdate();
            listBox.Items.Clear();

            var term = search.Trim().ToLowerInvariant();
            var filtered = _allFields
                .Where(f => string.IsNullOrEmpty(term) ||
                            f.Header.ToLowerInvariant().Contains(term) ||
                            f.Property.ToLowerInvariant().Contains(term));

            foreach (var item in filtered.OrderBy(x => x.Header))
            {
                listBox.Items.Add(new FieldListItem(item.Property, item.Header));
            }

            listBox.EndUpdate();
        }

        private void UpdatePreview()
        {
            _lblPreview.Text = _rootPanel.Group.IsEmpty
                ? "No filter defined - all records will be shown"
                : GroupToString(_rootPanel.Group);
        }

        private string GroupToString(FilterGroup group)
        {
            if (group.Children.Count == 0) return "";

            var parts = new List<string>();

            foreach (var node in group.Children)
            {
                if (node.IsGroup)
                {
                    string inner = GroupToString(node.Group);
                    if (!string.IsNullOrEmpty(inner))
                        parts.Add($"({inner})");
                }
                else
                {
                    var c = node.Condition;
                    string header = FindHeader(c.PropertyName);
                    parts.Add(c.ToDisplayString(header));
                }
            }

            string separator = group.Logic switch
            {
                LogicalOperator.And => " AND ",
                LogicalOperator.Or => " OR ",
                LogicalOperator.NotAnd => " AND NOT ",
                LogicalOperator.NotOr => " OR NOT ",
                _ => " AND "
            };

            return string.Join(separator, parts);
        }

        private string FindHeader(string propertyName)
        {
            var match = _allFields.FirstOrDefault(f => f.Property == propertyName);
            return match.Header ?? propertyName;
        }

        // 1. Apply Button: Applies filter and closes
        private void btnApply_Click(object sender, EventArgs e)
        {
            ResultFilter = _rootPanel.Group;
            DialogResult = DialogResult.OK;
            Close();
        }

        // 2. Cancel Button: Does nothing and closes
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    internal class FieldListItem
    {
        public string Property { get; }
        public string Header { get; }

        public FieldListItem(string property, string header)
        {
            Property = property;
            Header = header;
        }

        public override string ToString() => Header;
    }
}