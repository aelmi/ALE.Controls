using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ALE.Controls.Grouping;
using ALE.Controls.Theming;

namespace ALE.Controls
{
    public partial class DataGridEx
    {
        [Category("Behavior")]
        public string GroupByProperty
        {
            get => _groupProperty;
            set
            {
                _groupProperty = value;
                _collapsedGroups.Clear();
                UpdateGroupPanelUI();
                ApplySortAndFilter(); // Defined in Filtering.cs
            }
        }

        private void SetupGroupPanel()
        {
            _groupPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                AllowDrop = true,
                Padding = new Padding(8, 6, 8, 6)
            };

            _lblGroupHint = new Label
            {
                Text = "Drag a column header here to group by that column",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic)
            };

            _groupChipsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Visible = false
            };

            _groupPanel.Controls.Add(_lblGroupHint);
            _groupPanel.Controls.Add(_groupChipsPanel);

            _groupPanel.DragEnter += GroupPanel_DragEnter;
            _groupPanel.DragDrop += GroupPanel_DragDrop;

            Controls.Add(_groupPanel);
            _groupPanel.BringToFront();
        }

        private void UpdateGroupPanelUI()
        {
            if (_groupChipsPanel == null) return;

            _groupChipsPanel.Controls.Clear();

            if (string.IsNullOrEmpty(_groupProperty))
            {
                _lblGroupHint.Visible = true;
                _groupChipsPanel.Visible = false;
            }
            else
            {
                _lblGroupHint.Visible = false;
                _groupChipsPanel.Visible = true;

                string headerText = _propertyToHeader.ContainsKey(_groupProperty)
                    ? _propertyToHeader[_groupProperty]
                    : _groupProperty;

                var p = ThemePalette.GetPalette(_currentTheme);

                var chip = new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Height = 28,
                    WrapContents = false,
                    BackColor = p.Accent,
                    ForeColor = Color.White,
                    Margin = new Padding(0, 0, 8, 0),
                    Cursor = Cursors.SizeAll
                };

                var lbl = new Label
                {
                    Text = headerText,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Padding = new Padding(8, 4, 2, 4),
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Margin = new Padding(0)
                };

                var btnClose = new Button
                {
                    Text = "×",
                    Width = 24,
                    Height = 24,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(0, 2, 4, 0)
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, Color.Black);

                btnClose.Click += (s, e) => { GroupByProperty = null; };

                chip.Controls.Add(lbl);
                chip.Controls.Add(btnClose);

                MouseEventHandler startDrag = (s, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                        chip.DoDragDrop("UNGROUP_ACTION", DragDropEffects.Move);
                };
                chip.MouseDown += startDrag;
                lbl.MouseDown += startDrag;

                _groupChipsPanel.Controls.Add(chip);
            }
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = _grid.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.ColumnHeader && e.Button == MouseButtons.Left)
            {
                var col = _grid.Columns[hit.ColumnIndex];
                if (!string.IsNullOrEmpty(col.DataPropertyName))
                {
                    _draggedColumnName = col.DataPropertyName;
                    Size dragSize = SystemInformation.DragSize;
                    _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
            }
            else
            {
                _dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    _grid.DoDragDrop(_draggedColumnName, DragDropEffects.Move);
                }
            }
        }

        private void GroupPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && (string)e.Data.GetData(typeof(string)) != "UNGROUP_ACTION")
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void GroupPanel_DragDrop(object sender, DragEventArgs e)
        {
            string colName = (string)e.Data.GetData(typeof(string));
            if (!string.IsNullOrEmpty(colName) && colName != "UNGROUP_ACTION")
            {
                GroupByProperty = colName;
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && (string)e.Data.GetData(typeof(string)) == "UNGROUP_ACTION")
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Grid_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)) && (string)e.Data.GetData(typeof(string)) == "UNGROUP_ACTION")
            {
                GroupByProperty = null;
            }
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Left)
            {
                var item = _grid.Rows[e.RowIndex].DataBoundItem;
                if (item is GroupInfo group)
                {
                    string key = group.GroupValue?.ToString() ?? "None";
                    _collapsedGroups[key] = !_collapsedGroups.GetValueOrDefault(key, false);
                    ApplySortAndFilter();
                }
            }
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var item = _grid.Rows[e.RowIndex].DataBoundItem;
            if (item is GroupInfo group)
            {
                e.Handled = true;

                var p = ThemePalette.GetPalette(_currentTheme);
                Rectangle rowBounds = e.RowBounds;

                bool isSelected = _grid.Rows[e.RowIndex].Selected;
                Color backColor = isSelected ? p.SelectionBackground : p.HeaderBackground;
                Color foreColor = isSelected ? p.SelectionForeground : p.HeaderText;

                e.Graphics.FillRectangle(new SolidBrush(backColor), rowBounds);
                e.Graphics.DrawLine(new Pen(p.GridLines), rowBounds.Left, rowBounds.Bottom - 1, rowBounds.Right, rowBounds.Bottom - 1);

                string chevron = group.IsCollapsed ? "▶" : "▼";
                string text = $"  {chevron} {group.PropertyName}: {group.GroupValue}  ({group.ChildCount} items)";

                TextRenderer.DrawText(e.Graphics, text, new Font(_grid.Font, FontStyle.Bold),
                    new Point(rowBounds.Left + 10, rowBounds.Top + 8), foreColor);
            }
        }
    }
}