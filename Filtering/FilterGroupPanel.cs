using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ALE.Controls.Theming;

namespace ALE.Controls.Filtering
{
    public class FilterGroupPanel : UserControl
    {
        private readonly List<(string Property, string Header)> _columns;
        private readonly Type _dataSourceType;
        private readonly Panel _contentPanel;
        private readonly Button _btnLogic;
        private readonly ContextMenuStrip _logicMenu;
        private LogicalOperator _logic = LogicalOperator.And;
        private readonly GridTheme _theme;
        private readonly ThemeColors _colors;

        public FilterGroup Group { get; private set; }
        public event EventHandler FilterChanged;
        public event EventHandler RemoveGroupRequested;
        private readonly bool _isRoot;

        // --- DESIGN TIME SUPPORT ---
        public FilterGroupPanel()
            : this(
                new List<(string, string)> { ("Field1", "Field 1") },
                typeof(object),
                null,
                true,
                GridTheme.Light)
        {
        }

        public FilterGroupPanel(
            List<(string Property, string Header)> columns,
            Type dataSourceType,
            FilterGroup existingGroup = null,
            bool isRoot = false,
            GridTheme theme = GridTheme.Light)
        {
            _columns = columns;
            _dataSourceType = dataSourceType;
            _isRoot = isRoot;
            _theme = theme;
            _colors = ThemePalette.GetPalette(theme);
            Group = existingGroup ?? new FilterGroup();
            _logic = Group.Logic;

            Dock = DockStyle.Top;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(_isRoot ? 0 : 24, 8, 8, 8);
            BackColor = Color.Transparent;

            _btnLogic = new Button
            {
                Text = GetLogicDisplayText(_logic),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Size = new Size(90, 26),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0),
                BackColor = _colors.HeaderBackground,
                ForeColor = _colors.HeaderText
            };
            _btnLogic.FlatAppearance.BorderSize = 0;

            _logicMenu = new ContextMenuStrip();
            foreach (LogicalOperator op in Enum.GetValues(typeof(LogicalOperator)))
            {
                var item = new ToolStripMenuItem(GetLogicDisplayText(op))
                {
                    Tag = op,
                    Checked = op == _logic,
                    BackColor = _colors.RowEven,
                    ForeColor = _colors.TextPrimary
                };
                item.Click += (s, e) => SetLogic(op);
                _logicMenu.Items.Add(item);
            }

            _btnLogic.Click += (s, e) =>
            {
                _logicMenu.Show(_btnLogic, new Point(0, _btnLogic.Height));
            };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(_btnLogic);

            _contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                Padding = new Padding(_isRoot ? 0 : 12, 4, 0, 0)
            };

            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 6, 0, 0),
                BackColor = Color.Transparent
            };

            var btnAdd = CreateLinkLabel("Add Condition", Color.FromArgb(34, 139, 34));
            btnAdd.LinkClicked += (s, e) => AddConditionRow();

            var btnAddGroup = CreateLinkLabel("Add Group", Color.FromArgb(70, 130, 180));
            btnAddGroup.LinkClicked += (s, e) => AddNestedGroup();

            actionPanel.Controls.Add(btnAdd);
            actionPanel.Controls.Add(CreateSeparator());
            actionPanel.Controls.Add(btnAddGroup);

            if (!_isRoot)
            {
                actionPanel.Controls.Add(CreateSeparator());
                var btnRemove = CreateLinkLabel("Remove Group", Color.FromArgb(180, 60, 60));
                btnRemove.LinkClicked += (s, e) => RemoveGroupRequested?.Invoke(this, EventArgs.Empty);
                actionPanel.Controls.Add(btnRemove);
            }

            Controls.Add(actionPanel);
            Controls.Add(_contentPanel);
            Controls.Add(headerPanel);

            Paint += FilterGroupPanel_Paint;

            if (existingGroup != null && existingGroup.Children.Count > 0)
                RebuildFromGroup(existingGroup);
        }

        private string GetLogicDisplayText(LogicalOperator logic) =>
            logic switch
            {
                LogicalOperator.And => "AND",
                LogicalOperator.Or => "OR",
                LogicalOperator.NotAnd => "NOT AND",
                LogicalOperator.NotOr => "NOT OR",
                _ => "AND"
            };

        private void SetLogic(LogicalOperator newLogic)
        {
            _logic = newLogic;
            _btnLogic.Text = GetLogicDisplayText(_logic);

            foreach (ToolStripMenuItem item in _logicMenu.Items)
            {
                item.Checked = (LogicalOperator)item.Tag == newLogic;
            }

            RebuildGroup();
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private LinkLabel CreateLinkLabel(string text, Color color)
        {
            return new LinkLabel
            {
                Text = text,
                AutoSize = true,
                LinkColor = color,
                Font = new Font("Segoe UI", 8.25f),
                Padding = new Padding(0, 4, 8, 4),
                LinkBehavior = LinkBehavior.HoverUnderline,
                Margin = new Padding(0)
            };
        }

        private Label CreateSeparator()
        {
            return new Label
            {
                Text = "|",
                AutoSize = true,
                ForeColor = Color.FromArgb(180, 180, 180),
                Font = new Font("Segoe UI", 8.25f),
                Margin = new Padding(4, 4, 4, 4)
            };
        }

        public void AddConditionForProperty(string propertyName)
        {
            var cond = new FilterCondition
            {
                PropertyName = propertyName,
                Operator = FilterOperator.Contains,
                Value = ""
            };
            AddConditionRow(cond);
            RebuildGroup();
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RebuildFromGroup(FilterGroup group)
        {
            _contentPanel.Controls.Clear();
            _logic = group.Logic;
            _btnLogic.Text = GetLogicDisplayText(_logic);

            foreach (var item in _logicMenu.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                    menuItem.Checked = (LogicalOperator)menuItem.Tag == _logic;
            }

            foreach (var node in group.Children)
            {
                if (node.IsGroup)
                    AddNestedGroup(node.Group);
                else
                    AddConditionRow(node.Condition);
            }
            RebuildGroup();
        }

        public void AddConditionRow(FilterCondition existing = null)
        {
            var row = new FilterRowControl(_columns, _dataSourceType, existing, _theme);
            row.ConditionChanged += (s, e) => { RebuildGroup(); FilterChanged?.Invoke(this, EventArgs.Empty); };
            row.RemoveClicked += (s, e) => {
                _contentPanel.Controls.Remove((Control)s);
                RebuildGroup();
                FilterChanged?.Invoke(this, EventArgs.Empty);
            };

            _contentPanel.Controls.Add(row);

            RebuildGroup();
        }

        private void AddNestedGroup(FilterGroup existing = null)
        {
            var panel = new FilterGroupPanel(_columns, _dataSourceType, existing, false, _theme);
            panel.FilterChanged += (s, e) => { RebuildGroup(); FilterChanged?.Invoke(this, EventArgs.Empty); };
            panel.RemoveGroupRequested += (s, e) => {
                _contentPanel.Controls.Remove((Control)s);
                RebuildGroup();
                FilterChanged?.Invoke(this, EventArgs.Empty);
            };
            _contentPanel.Controls.Add(panel);
            RebuildGroup();
        }

        private void RebuildGroup()
        {
            Group.Logic = _logic;
            Group.Children.Clear();
            foreach (Control c in _contentPanel.Controls)
            {
                if (c is FilterRowControl row)
                    Group.Children.Add(FilterNode.FromCondition(row.Condition));
                else if (c is FilterGroupPanel gp)
                    Group.Children.Add(FilterNode.FromGroup(gp.Group));
            }
        }

        private void FilterGroupPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_isRoot) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using var pen = new Pen(_colors.HeaderBackground, 2f);
            int indent = 8;
            int top = 0;
            int bottom = Height - 8;
            int right = indent + 16;

            g.DrawLine(pen, indent, top, indent, bottom);
            g.DrawLine(pen, indent, top, right, top);
            g.DrawLine(pen, indent, bottom, right, bottom);
        }
    }
}