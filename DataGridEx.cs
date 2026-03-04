using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ALE.Controls.Filtering;
using ALE.Controls.Theming;
using ALE.Controls.Grouping;

namespace ALE.Controls
{
    public partial class DataGridEx : UserControl
    {
        // ==========================================
        // ALL PRIVATE FIELDS 
        // ==========================================
        private DataGridView _grid;
        private List<object> _sourceData = new();
        private Type _itemType;
        private BindingList<object> _bindingList;

        private ToolStrip _toolStrip;
        private ToolStripDropDownButton _btnTheme;
        private ToolStripDropDownButton _btnColumns;
        private ToolStripButton _btnExport;
        private ToolStripLabel _lblFilterStatus;
        private ToolStripButton _btnClearFilter;
        private ToolStripButton _btnAdvancedFilter;
        private GridTheme _currentTheme = GridTheme.Clean;
        private bool _showThemeSelector = false;
        private bool _showColumnSelector = true; // NEW FIELD
        private int _hoveredRowIndex = -1;

        private ToolStripTextBox _txtGlobalSearch;
        private ToolStripLabel _lblSearchIcon;
        private System.Windows.Forms.Timer _searchDebounceTimer;
        private bool _showGlobalSearch = true;

        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _mnuAutoSize;
        private ToolStripSeparator _customMenuSeparator;
        private bool _showContextMenu = true;
        private int _rightClickRowIndex = -1;
        private int _rightClickColIndex = -1;
        private readonly Dictionary<string, int> _originalColumnWidths = new();

        private Panel _groupPanel;
        private Label _lblGroupHint;
        private FlowLayoutPanel _groupChipsPanel;
        private Rectangle _dragBoxFromMouseDown;
        private string _draggedColumnName;
        private readonly List<string> _groupProperties = new();
        private readonly Dictionary<string, bool> _collapsedGroups = new();

        private FilterGroup _activeFilter;
        private string _sortProperty;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private readonly Dictionary<string, string> _propertyToHeader = new(StringComparer.OrdinalIgnoreCase);

        private int _rowHeight = 36;
        private int _searchBoxWidth = 250;

        public event EventHandler GridSelectionChanged;
        public event EventHandler<object> ItemDoubleClicked;

        [Browsable(false)] public DataGridView Grid => _grid;
        [Browsable(false)] public IReadOnlyDictionary<string, string> PropertyHeaders => _propertyToHeader;

        [Category("Appearance")]
        [DefaultValue(36)]
        [Description("Determines the height of the data rows in the grid.")]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                if (_grid != null)
                {
                    _grid.RowTemplate.Height = value;
                    foreach (DataGridViewRow row in _grid.Rows) row.Height = value;
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(250)]
        [Description("Determines the width of the global search box in the toolbar.")]
        public int SearchBoxWidth
        {
            get => _searchBoxWidth;
            set
            {
                _searchBoxWidth = value;
                if (_txtGlobalSearch != null)
                {
                    _txtGlobalSearch.Width = value;
                }
            }
        }

        [Category("Appearance")]
        public GridTheme Theme { get => _currentTheme; set { _currentTheme = value; ApplyTheme(); } }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowThemeSelector { get => _showThemeSelector; set { _showThemeSelector = value; if (_btnTheme != null) _btnTheme.Visible = value; } }

        // NEW PROPERTY
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Toggles the visibility of the Columns dropdown menu in the toolbar.")]
        public bool ShowColumnSelector
        {
            get => _showColumnSelector;
            set { _showColumnSelector = value; if (_btnColumns != null) _btnColumns.Visible = value; }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowGlobalSearch { get => _showGlobalSearch; set { _showGlobalSearch = value; if (_txtGlobalSearch != null) _txtGlobalSearch.Visible = value; if (_lblSearchIcon != null) _lblSearchIcon.Visible = value; } }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowContextMenu { get => _showContextMenu; set => _showContextMenu = value; }

        // ==========================================
        // INITIALIZATION
        // ==========================================
        public DataGridEx()
        {
            InitializeComponent();

            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            SetupToolbarAndTheme();
            SetupGroupPanel();
            SetupContextMenu();
            SetupGrid();
            ApplyTheme();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) SetupDesignTimeAppearance();
        }

        // ==========================================
        // KEYBOARD SHORTCUTS
        // ==========================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F) && _showGlobalSearch && _txtGlobalSearch != null)
            {
                _txtGlobalSearch.Focus();
                _txtGlobalSearch.SelectAll();
                return true;
            }
            if (keyData == (Keys.Control | Keys.C))
            {
                CopyCellValue();
                return true;
            }
            if (keyData == (Keys.Control | Keys.E))
            {
                ExportToCsv();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ==========================================
        // EXTENSIBLE CONTEXT MENU
        // ==========================================
        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            _customMenuSeparator = new ToolStripSeparator { Visible = false };
            _contextMenu.Items.Add(_customMenuSeparator);

            var mnuCopyCell = new ToolStripMenuItem("📋 Copy Cell");
            mnuCopyCell.Click += (s, e) => CopyCell();

            var mnuCopyRow = new ToolStripMenuItem("📄 Copy Row");
            mnuCopyRow.Click += (s, e) => CopyRow();

            var mnuExport = new ToolStripMenuItem("📥 Export Selected");
            mnuExport.Click += (s, e) => ExportToCsv(true);

            _mnuAutoSize = new ToolStripMenuItem("⚙ Auto-size Columns") { CheckOnClick = true };
            _mnuAutoSize.Click += (s, e) => ToggleAutoSizeColumns();

            _contextMenu.Items.AddRange(new ToolStripItem[] { mnuCopyCell, mnuCopyRow, new ToolStripSeparator(), mnuExport, new ToolStripSeparator(), _mnuAutoSize });
        }

        public void AddContextMenuItem(string text, EventHandler onClick, Keys shortcutKeys = Keys.None)
        {
            var item = new ToolStripMenuItem(text);
            if (shortcutKeys != Keys.None) item.ShortcutKeys = shortcutKeys;
            item.Click += onClick;

            int insertIndex = _contextMenu.Items.IndexOf(_customMenuSeparator);
            _contextMenu.Items.Insert(insertIndex, item);
            _customMenuSeparator.Visible = true;
        }

        // ==========================================
        // EXPORT ENGINE
        // ==========================================
        public void ExportToCsv(bool selectedOnly = false)
        {
            var rowsToExport = selectedOnly ? _grid.SelectedRows.Cast<DataGridViewRow>().ToList() : _grid.Rows.Cast<DataGridViewRow>().ToList();
            if (rowsToExport.Count == 0) { MessageBox.Show("No data available to export.", "Export Empty", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using var sfd = new SaveFileDialog { Filter = "CSV Files|*.csv", FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();
                    var visibleCols = _grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();

                    sb.AppendLine(string.Join(",", visibleCols.Select(c => $"\"{c.HeaderText}\"")));

                    foreach (var row in rowsToExport)
                    {
                        if (row.DataBoundItem is GroupInfo) continue;
                        var values = visibleCols.Select(c => $"\"{row.Cells[c.Index].Value?.ToString().Replace("\"", "\"\"") ?? ""}\"");
                        sb.AppendLine(string.Join(",", values));
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Successfully exported {rowsToExport.Count(r => !(r.DataBoundItem is GroupInfo))} rows.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export data: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==========================================
        // GRID INITIALIZATION
        // ==========================================
        private void SetupGrid()
        {
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowDrop = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = _rowHeight }
            };

            _grid.SelectionChanged += (_, _) => GridSelectionChanged?.Invoke(this, EventArgs.Empty);
            _grid.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && !(_grid.Rows[e.RowIndex].DataBoundItem is GroupInfo))
                    ItemDoubleClicked?.Invoke(this, _grid.Rows[e.RowIndex].DataBoundItem);
            };
            _grid.CellMouseEnter += (s, e) => { if (e.RowIndex >= 0 && e.RowIndex != _hoveredRowIndex) { int old = _hoveredRowIndex; _hoveredRowIndex = e.RowIndex; UpdateRowStyle(old); UpdateRowStyle(_hoveredRowIndex); } };
            _grid.CellMouseLeave += (s, e) => { if (_hoveredRowIndex >= 0) { int old = _hoveredRowIndex; _hoveredRowIndex = -1; UpdateRowStyle(old); } };

            _grid.CellMouseDown += Grid_CellMouseDown;
            _grid.MouseDown += Grid_MouseDown;
            _grid.MouseMove += Grid_MouseMove;
            _grid.DragEnter += Grid_DragEnter;
            _grid.DragDrop += Grid_DragDrop;
            _grid.CellMouseClick += Grid_CellMouseClick;
            _grid.RowPrePaint += Grid_RowPrePaint;
            _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;

            Controls.Add(_grid);
            _grid.BringToFront();
        }

        private void Grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _showContextMenu)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    _rightClickRowIndex = e.RowIndex;
                    _rightClickColIndex = e.ColumnIndex;
                    if (!_grid.Rows[e.RowIndex].Selected)
                    {
                        _grid.ClearSelection();
                        _grid.Rows[e.RowIndex].Selected = true;
                    }
                    bool isDataRow = !(_grid.Rows[e.RowIndex].DataBoundItem is GroupInfo);
                    _contextMenu.Items[1].Enabled = isDataRow;
                    _contextMenu.Items[2].Enabled = isDataRow;
                    _contextMenu.Show(Cursor.Position);
                }
                else if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    _rightClickRowIndex = -1;
                    _rightClickColIndex = e.ColumnIndex;
                    _contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void CopyCell() { if (_rightClickRowIndex >= 0 && _rightClickColIndex >= 0) { var val = _grid.Rows[_rightClickRowIndex].Cells[_rightClickColIndex].Value; if (val != null) Clipboard.SetText(val.ToString()); } }
        private void CopyRow() { if (_rightClickRowIndex >= 0) { var row = _grid.Rows[_rightClickRowIndex]; var values = new List<string>(); foreach (DataGridViewCell cell in row.Cells) if (cell.Visible) values.Add(cell.Value?.ToString() ?? ""); if (values.Count > 0) Clipboard.SetText(string.Join("\t", values)); } }
        private void CopyCellValue() { if (_grid.CurrentCell?.Value != null && !(_grid.CurrentRow?.DataBoundItem is GroupInfo)) Clipboard.SetText(_grid.CurrentCell.Value.ToString()); }
        private void ToggleAutoSizeColumns() { if (_mnuAutoSize.Checked) { _originalColumnWidths.Clear(); foreach (DataGridViewColumn col in _grid.Columns) _originalColumnWidths[col.Name] = col.Width; _grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells); } else { foreach (DataGridViewColumn col in _grid.Columns) if (_originalColumnWidths.TryGetValue(col.Name, out int w)) col.Width = w; } }

        public List<T> GetSelectedItems<T>() where T : class
        {
            var items = new List<T>();
            foreach (DataGridViewRow row in _grid.Rows) if (row.Selected && row.DataBoundItem is T item) items.Add(item);
            return items;
        }

        public void AddGroupProperty(string propertyName)
        {
            if (!_groupProperties.Contains(propertyName))
            {
                _groupProperties.Add(propertyName);
                _collapsedGroups.Clear();
                UpdateGroupPanelUI();
                ApplySortAndFilter();
            }
        }

        // ==========================================
        // TOOLBAR & THEMING
        // ==========================================
        private void SetupToolbarAndTheme()
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 38 };
            _toolStrip = new ToolStrip { Dock = DockStyle.Fill, GripStyle = ToolStripGripStyle.Hidden, Renderer = new ToolStripProfessionalRenderer() };

            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); ApplySortAndFilter(); };

            _lblSearchIcon = new ToolStripLabel("🔍");
            _lblSearchIcon.Font = new Font("Segoe UI", 10F);
            _lblSearchIcon.ForeColor = Color.FromArgb(156, 163, 175);

            _txtGlobalSearch = new ToolStripTextBox { Width = _searchBoxWidth, Text = "Search..." };
            _txtGlobalSearch.ForeColor = Color.Gray;
            _txtGlobalSearch.GotFocus += (s, e) => { if (_txtGlobalSearch.Text == "Search...") { _txtGlobalSearch.Text = ""; _txtGlobalSearch.ForeColor = ThemePalette.GetPalette(_currentTheme).TextPrimary; } };
            _txtGlobalSearch.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(_txtGlobalSearch.Text)) { _txtGlobalSearch.Text = "Search..."; _txtGlobalSearch.ForeColor = Color.Gray; } };
            _txtGlobalSearch.TextChanged += (s, e) => { if (_txtGlobalSearch.Text != "Search...") { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); } };

            _lblSearchIcon.Visible = _showGlobalSearch;
            _txtGlobalSearch.Visible = _showGlobalSearch;

            _btnColumns = new ToolStripDropDownButton("⚙ Columns");
            _btnColumns.Font = new Font("Segoe UI", 10F);
            _btnColumns.ForeColor = Color.FromArgb(156, 163, 175);
            _btnColumns.Visible = _showColumnSelector; // NEW BOUND VISIBILITY

            _btnColumns.DropDownOpening += (s, e) =>
            {
                _btnColumns.DropDownItems.Clear();
                foreach (DataGridViewColumn col in _grid.Columns)
                {
                    var item = new ToolStripMenuItem(col.HeaderText) { Checked = col.Visible, CheckOnClick = true, Tag = col };
                    item.CheckedChanged += (s2, e2) => ((DataGridViewColumn)((ToolStripMenuItem)s2).Tag).Visible = ((ToolStripMenuItem)s2).Checked;
                    _btnColumns.DropDownItems.Add(item);
                }
            };

            _btnExport = new ToolStripButton("📥 Export", null, (s, e) => ExportToCsv());
            _btnExport.Font = new Font("Segoe UI", 10F);
            _btnExport.ForeColor = Color.FromArgb(156, 163, 175);

            _btnAdvancedFilter = new ToolStripButton("Advanced Filter...", null, (_, _) => ShowFilterBuilder());
            _btnAdvancedFilter.Font = new Font("Segoe UI", 10F);
            _btnAdvancedFilter.ForeColor = Color.FromArgb(156, 163, 175);

            _btnClearFilter = new ToolStripButton("Clear", null, (_, _) => ClearFilter()) { Visible = false };
            _lblFilterStatus = new ToolStripLabel("No filter") { ForeColor = Color.Gray };

            _btnTheme = new ToolStripDropDownButton("Theme");
            foreach (GridTheme t in Enum.GetValues(typeof(GridTheme))) _btnTheme.DropDownItems.Add(new ToolStripMenuItem(t.ToString(), null, (_, _) => Theme = t));
            _btnTheme.Visible = _showThemeSelector;

            _toolStrip.Items.AddRange(new ToolStripItem[] { _lblSearchIcon, _txtGlobalSearch, new ToolStripSeparator(), _btnColumns, _btnExport, new ToolStripSeparator(), _btnAdvancedFilter, _btnClearFilter, new ToolStripSeparator(), _lblFilterStatus, new ToolStripControlHost(new Label { Width = 20 }), _btnTheme });
            topPanel.Controls.Add(_toolStrip); Controls.Add(topPanel);
        }

        private void ApplyTheme()
        {
            if (_grid == null) return;

            var p = ThemePalette.GetPalette(_currentTheme);
            if (_groupPanel != null) { _groupPanel.BackColor = p.ToolbarBackground; _groupPanel.ForeColor = p.TextPrimary; UpdateGroupPanelUI(); }
            if (_contextMenu != null) { _contextMenu.BackColor = p.ToolbarBackground; _contextMenu.ForeColor = p.TextPrimary; }
            if (_txtGlobalSearch != null) { _txtGlobalSearch.BackColor = p.RowEven; if (_txtGlobalSearch.Text != "Search...") _txtGlobalSearch.ForeColor = p.TextPrimary; }

            Font headerFont = new Font(this.Font.FontFamily, this.Font.Size + 0.5f, FontStyle.Bold);
            Padding cellPadding = new Padding(8, 0, 8, 0);

            if (_currentTheme == GridTheme.None)
            {
                _grid.EnableHeadersVisualStyles = true; _grid.BackgroundColor = SystemColors.Window;
                _grid.DefaultCellStyle.BackColor = SystemColors.Window; _grid.DefaultCellStyle.ForeColor = SystemColors.WindowText;
                _grid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight; _grid.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                _grid.DefaultCellStyle.Padding = cellPadding;

                _grid.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window; _grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight; _grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                _grid.AlternatingRowsDefaultCellStyle.Padding = cellPadding;

                _grid.ColumnHeadersHeight = 42;
                _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = SystemColors.Control, ForeColor = SystemColors.WindowText, Font = headerFont, Padding = new Padding(8, 6, 8, 6), Alignment = DataGridViewContentAlignment.MiddleLeft };
                _grid.GridColor = SystemColors.ControlDark; BackColor = SystemColors.Control;
            }
            else
            {
                _grid.EnableHeadersVisualStyles = false; _grid.BackgroundColor = p.GridBackground; _grid.GridColor = p.GridLines;
                _grid.DefaultCellStyle.BackColor = p.RowEven; _grid.DefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.DefaultCellStyle.SelectionBackColor = p.SelectionBackground; _grid.DefaultCellStyle.SelectionForeColor = p.SelectionForeground;
                _grid.DefaultCellStyle.Padding = cellPadding;

                _grid.AlternatingRowsDefaultCellStyle.BackColor = p.RowOdd; _grid.AlternatingRowsDefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = p.SelectionBackground; _grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = p.SelectionForeground;
                _grid.AlternatingRowsDefaultCellStyle.Padding = cellPadding;

                _grid.ColumnHeadersHeight = 42;
                _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = p.HeaderBackground, ForeColor = p.HeaderText, SelectionBackColor = p.HeaderBackground, SelectionForeColor = p.HeaderText, Font = headerFont, Padding = new Padding(8, 6, 8, 6), Alignment = DataGridViewContentAlignment.MiddleLeft };
                BackColor = p.ToolbarBackground;
            }

            if (_hoveredRowIndex >= 0 && _hoveredRowIndex < _grid.RowCount) UpdateRowStyle(_hoveredRowIndex);
            _grid.Invalidate();
        }

        private void UpdateRowStyle(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.RowCount) return;
            if (_grid.Rows[rowIndex].DataBoundItem is GroupInfo) return;

            if (rowIndex == _hoveredRowIndex)
            {
                var p = ThemePalette.GetPalette(_currentTheme);
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = p.RowHover; _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = p.TextPrimary; _grid.Rows[rowIndex].DefaultCellStyle.SelectionBackColor = p.SelectionBackground; _grid.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = p.SelectionForeground;
            }
            else
            {
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Empty; _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Empty; _grid.Rows[rowIndex].DefaultCellStyle.SelectionBackColor = Color.Empty; _grid.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = Color.Empty;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            ApplyTheme();
            if (_lblGroupHint != null) _lblGroupHint.Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Italic);
            UpdateGroupPanelUI();
        }

        private void SetupDesignTimeAppearance() { _grid.Columns.Add("C1", "C1"); _grid.Rows.Add("Data"); _grid.ClearSelection(); }
    }
}