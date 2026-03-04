using System;
using System.Drawing;
using System.Windows.Forms;
using ALE.Controls.Grouping;
using ALE.Controls.Theming;

namespace ALE.Controls
{
    public partial class DataGridEx
    {
        private void SetupGroupPanel()
        {
            _groupPanel = new Panel { Dock = DockStyle.Top, Height = 40, AllowDrop = true, Padding = new Padding(8, 6, 8, 6) };
            _lblGroupHint = new Label { Text = "Drag a column header here to group by that column", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.Gray, Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Italic) };
            _groupChipsPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Visible = false };

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

            if (_groupProperties.Count == 0)
            {
                _lblGroupHint.Visible = true;
                _groupChipsPanel.Visible = false;
            }
            else
            {
                _lblGroupHint.Visible = false;
                _groupChipsPanel.Visible = true;
                var p = ThemePalette.GetPalette(_currentTheme);

                for (int i = 0; i < _groupProperties.Count; i++)
                {
                    string prop = _groupProperties[i];
                    string headerText = _propertyToHeader.ContainsKey(prop) ? _propertyToHeader[prop] : prop;

                    var chip = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Height = 28, WrapContents = false, BackColor = p.Accent, ForeColor = Color.White, Margin = new Padding(0, 0, 8, 0), Cursor = Cursors.SizeAll };
                    var lbl = new Label { Text = headerText, AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(8, 4, 2, 4), Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), Margin = new Padding(0) };
                    var btnClose = new Button { Text = "×", Width = 24, Height = 24, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font(this.Font.FontFamily, this.Font.Size + 1f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter, Margin = new Padding(0, 2, 4, 0) };

                    btnClose.FlatAppearance.BorderSize = 0; btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, Color.Black);

                    string propToRemove = prop;
                    btnClose.Click += (s, e) => { _groupProperties.Remove(propToRemove); _collapsedGroups.Clear(); UpdateGroupPanelUI(); ApplySortAndFilter(); };

                    chip.Controls.Add(lbl); chip.Controls.Add(btnClose);
                    MouseEventHandler startDrag = (s, e) => { if (e.Button == MouseButtons.Left) chip.DoDragDrop($"UNGROUP:{propToRemove}", DragDropEffects.Move); };
                    chip.MouseDown += startDrag; lbl.MouseDown += startDrag;

                    _groupChipsPanel.Controls.Add(chip);
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = _grid.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.ColumnHeader && e.Button == MouseButtons.Left)
            {
                // Fix: Check if mouse is near the left/right edge to allow native column resizing
                Rectangle headerRect = _grid.GetCellDisplayRectangle(hit.ColumnIndex, -1, false);
                int resizeMargin = 8;
                bool isResizing = (e.X <= headerRect.Left + resizeMargin) || (e.X >= headerRect.Right - resizeMargin);

                if (!isResizing)
                {
                    var col = _grid.Columns[hit.ColumnIndex];
                    if (!string.IsNullOrEmpty(col.DataPropertyName))
                    {
                        _draggedColumnName = col.DataPropertyName;
                        Size dragSize = SystemInformation.DragSize;
                        _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                        return; // Valid drag setup complete
                    }
                }
            }

            // Clear drag box if resizing or clicking elsewhere
            _dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                    _grid.DoDragDrop(_draggedColumnName, DragDropEffects.Move);
        }

        private void GroupPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string))) { string data = (string)e.Data.GetData(typeof(string)); if (!data.StartsWith("UNGROUP:")) e.Effect = DragDropEffects.Move; else e.Effect = DragDropEffects.None; }
        }

        private void GroupPanel_DragDrop(object sender, DragEventArgs e)
        {
            string colName = (string)e.Data.GetData(typeof(string));
            if (!string.IsNullOrEmpty(colName) && !colName.StartsWith("UNGROUP:"))
            {
                if (!_groupProperties.Contains(colName)) { _groupProperties.Add(colName); _collapsedGroups.Clear(); UpdateGroupPanelUI(); ApplySortAndFilter(); }
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string))) { string data = (string)e.Data.GetData(typeof(string)); if (data.StartsWith("UNGROUP:")) e.Effect = DragDropEffects.Move; else e.Effect = DragDropEffects.None; }
        }

        private void Grid_DragDrop(object sender, DragEventArgs e)
        {
            string data = (string)e.Data.GetData(typeof(string));
            if (data != null && data.StartsWith("UNGROUP:"))
            {
                string propToRemove = data.Replace("UNGROUP:", "");
                _groupProperties.Remove(propToRemove); _collapsedGroups.Clear(); UpdateGroupPanelUI(); ApplySortAndFilter();
            }
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.Button == MouseButtons.Left)
            {
                if (_grid.Rows[e.RowIndex].DataBoundItem is GroupInfo group)
                {
                    _collapsedGroups[group.GroupPath] = !_collapsedGroups.GetValueOrDefault(group.GroupPath, false);
                    ApplySortAndFilter();
                }
            }
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (_grid.Rows[e.RowIndex].DataBoundItem is GroupInfo group)
            {
                e.Handled = true;
                var p = ThemePalette.GetPalette(_currentTheme);
                Rectangle r = e.RowBounds;
                bool isSelected = _grid.Rows[e.RowIndex].Selected;
                Color backColor = isSelected ? p.SelectionBackground : p.HeaderBackground;
                Color foreColor = isSelected ? p.SelectionForeground : p.HeaderText;

                if (group.Level > 0 && !isSelected) backColor = ControlPaint.Light(backColor, 0.5f + (group.Level * 0.1f));

                e.Graphics.FillRectangle(new SolidBrush(backColor), r);
                e.Graphics.DrawLine(new Pen(p.GridLines), r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);

                string chevron = group.IsCollapsed ? "▶" : "▼";
                string text = $"  {chevron} {group.PropertyName}: {group.GroupValue}  ({group.ChildCount} items)";
                int indent = 10 + (group.Level * 20);

                TextRenderer.DrawText(e.Graphics, text, new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold), new Point(r.Left + indent, r.Top + 8), foreColor);
            }
        }
    }
}