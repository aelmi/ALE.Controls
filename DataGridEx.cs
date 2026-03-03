using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ALE.Controls.Filtering;
using ALE.Controls.Theming;
using ALE.Controls.Grouping;

namespace ALE.Controls
{
    public partial class DataGridEx : UserControl
    {
        // ==========================================
        // ALL PRIVATE FIELDS (DECLARED ONLY HERE)
        // ==========================================
        private DataGridView _grid;
        private List<object> _sourceData = new();
        private Type _itemType;
        private BindingList<object> _bindingList;

        private ToolStrip _toolStrip;
        private ToolStripDropDownButton _btnTheme;
        private ToolStripLabel _lblFilterStatus;
        private ToolStripButton _btnClearFilter;
        private ToolStripButton _btnAdvancedFilter;
        private GridTheme _currentTheme = GridTheme.Clean;
        private bool _showThemeSelector = false;
        private int _hoveredRowIndex = -1;

        // Context Menu Fields
        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _mnuAutoSize;
        private bool _showContextMenu = true;
        private int _rightClickRowIndex = -1;
        private int _rightClickColIndex = -1;
        private readonly Dictionary<string, int> _originalColumnWidths = new();

        private Panel _groupPanel;
        private Label _lblGroupHint;
        private FlowLayoutPanel _groupChipsPanel;
        private Rectangle _dragBoxFromMouseDown;
        private string _draggedColumnName;

        // MULTI-LEVEL GROUPING STATE
        private readonly List<string> _groupProperties = new();
        private readonly Dictionary<string, bool> _collapsedGroups = new();

        private FilterGroup _activeFilter;
        private string _sortProperty;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        private readonly Dictionary<string, string> _propertyToHeader = new(StringComparer.OrdinalIgnoreCase);

        // ==========================================
        // EVENTS & CORE PROPERTIES
        // ==========================================
        public event EventHandler GridSelectionChanged;
        public event EventHandler<object> ItemDoubleClicked;

        [Browsable(false)]
        public DataGridView Grid => _grid;

        [Category("Appearance")]
        public GridTheme Theme
        {
            get => _currentTheme;
            set { _currentTheme = value; ApplyTheme(); }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowThemeSelector
        {
            get => _showThemeSelector;
            set
            {
                _showThemeSelector = value;
                if (_btnTheme != null) _btnTheme.Visible = value;
            }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Determines if the right-click context menu is available.")]
        public bool ShowContextMenu
        {
            get => _showContextMenu;
            set => _showContextMenu = value;
        }

        [Browsable(false)]
        public IReadOnlyDictionary<string, string> PropertyHeaders => _propertyToHeader;

        // Method to assign grouping programmatically (e.g. from TestForm)
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
        // INITIALIZATION
        // ==========================================
        public DataGridEx()
        {
            InitializeComponent();

            SetupToolbarAndTheme();
            SetupGroupPanel();
            SetupContextMenu();
            SetupGrid();

            ApplyTheme();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                SetupDesignTimeAppearance();
            }
        }

        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var mnuCopyCell = new ToolStripMenuItem("Copy Cell");
            mnuCopyCell.Click += (s, e) => CopyCell();

            var mnuCopyRow = new ToolStripMenuItem("Copy Row");
            mnuCopyRow.Click += (s, e) => CopyRow();

            _mnuAutoSize = new ToolStripMenuItem("Auto-size Columns") { CheckOnClick = true };
            _mnuAutoSize.Click += (s, e) => ToggleAutoSizeColumns();

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                mnuCopyCell, mnuCopyRow, new ToolStripSeparator(), _mnuAutoSize
            });
        }

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
            };

            _grid.SelectionChanged += (_, _) => GridSelectionChanged?.Invoke(this, EventArgs.Empty);

            _grid.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem;
                    if (!(item is GroupInfo)) ItemDoubleClicked?.Invoke(this, item);
                }
            };

            _grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex != _hoveredRowIndex)
                {
                    int old = _hoveredRowIndex;
                    _hoveredRowIndex = e.RowIndex;
                    UpdateRowStyle(old);
                    UpdateRowStyle(_hoveredRowIndex);
                }
            };

            _grid.CellMouseLeave += (s, e) =>
            {
                if (_hoveredRowIndex >= 0)
                {
                    int old = _hoveredRowIndex;
                    _hoveredRowIndex = -1;
                    UpdateRowStyle(old);
                }
            };

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

        // ==========================================
        // CONTEXT MENU LOGIC
        // ==========================================
        private void Grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _showContextMenu)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    _rightClickRowIndex = e.RowIndex;
                    _rightClickColIndex = e.ColumnIndex;

                    _grid.ClearSelection();
                    _grid.Rows[e.RowIndex].Selected = true;

                    var item = _grid.Rows[e.RowIndex].DataBoundItem;
                    bool isDataRow = !(item is GroupInfo);
                    _contextMenu.Items[0].Enabled = isDataRow;
                    _contextMenu.Items[1].Enabled = isDataRow;

                    _contextMenu.Show(Cursor.Position);
                }
                else if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    _rightClickRowIndex = -1;
                    _rightClickColIndex = e.ColumnIndex;
                    _contextMenu.Items[0].Enabled = false;
                    _contextMenu.Items[1].Enabled = false;
                    _contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void CopyCell()
        {
            if (_rightClickRowIndex >= 0 && _rightClickColIndex >= 0)
            {
                var val = _grid.Rows[_rightClickRowIndex].Cells[_rightClickColIndex].Value;
                if (val != null) Clipboard.SetText(val.ToString());
            }
        }

        private void CopyRow()
        {
            if (_rightClickRowIndex >= 0)
            {
                var row = _grid.Rows[_rightClickRowIndex];
                var values = new List<string>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Visible) values.Add(cell.Value?.ToString() ?? "");
                }
                if (values.Count > 0) Clipboard.SetText(string.Join("\t", values));
            }
        }

        private void ToggleAutoSizeColumns()
        {
            if (_mnuAutoSize.Checked)
            {
                _originalColumnWidths.Clear();
                foreach (DataGridViewColumn col in _grid.Columns)
                    _originalColumnWidths[col.Name] = col.Width;

                _grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            else
            {
                foreach (DataGridViewColumn col in _grid.Columns)
                {
                    if (_originalColumnWidths.TryGetValue(col.Name, out int originalWidth))
                        col.Width = originalWidth;
                }
            }
        }

        // ==========================================
        // PUBLIC METHODS (UTILITY)
        // ==========================================
        public List<T> GetSelectedItems<T>() where T : class
        {
            var items = new List<T>();
            // Iterate through all rows in visual order, picking out the selected ones
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Selected && row.DataBoundItem is T item)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        // ==========================================
        // TOOLBAR & THEMING
        // ==========================================
        private void SetupToolbarAndTheme()
        {
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 36 };
            _toolStrip = new ToolStrip
            {
                Dock = DockStyle.Fill,
                GripStyle = ToolStripGripStyle.Hidden,
                Renderer = new ToolStripProfessionalRenderer()
            };

            _btnAdvancedFilter = new ToolStripButton("Advanced Filter...", null, (_, _) => ShowFilterBuilder());
            _btnClearFilter = new ToolStripButton("Clear", null, (_, _) => ClearFilter()) { Visible = false };
            _lblFilterStatus = new ToolStripLabel("No filter") { ForeColor = Color.Gray };
            _btnTheme = new ToolStripDropDownButton("Theme");

            foreach (GridTheme t in Enum.GetValues(typeof(GridTheme)))
                _btnTheme.DropDownItems.Add(new ToolStripMenuItem(t.ToString(), null, (_, _) => Theme = t));

            _btnTheme.Visible = _showThemeSelector;

            _toolStrip.Items.AddRange(new ToolStripItem[]
            {
                _btnAdvancedFilter, _btnClearFilter, new ToolStripSeparator(),
                _lblFilterStatus, new ToolStripControlHost(new Label { Width = 20 }), _btnTheme
            });

            topPanel.Controls.Add(_toolStrip);
            Controls.Add(topPanel);
        }

        private void ApplyTheme()
        {
            var p = ThemePalette.GetPalette(_currentTheme);

            if (_groupPanel != null)
            {
                _groupPanel.BackColor = p.ToolbarBackground;
                _groupPanel.ForeColor = p.TextPrimary;
                UpdateGroupPanelUI();
            }

            if (_contextMenu != null)
            {
                _contextMenu.BackColor = p.ToolbarBackground;
                _contextMenu.ForeColor = p.TextPrimary;
            }

            if (_currentTheme == GridTheme.None)
            {
                _grid.EnableHeadersVisualStyles = true;
                _grid.BackgroundColor = SystemColors.Window;
                _grid.DefaultCellStyle.BackColor = SystemColors.Window;
                _grid.DefaultCellStyle.ForeColor = SystemColors.WindowText;
                _grid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                _grid.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;
                _grid.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Window;
                _grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                _grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = SystemColors.Control,
                    ForeColor = SystemColors.WindowText,
                    Font = new Font("Segoe UI Semibold", 9.75f),
                    Padding = new Padding(8, 6, 8, 6),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                };
                _grid.GridColor = SystemColors.ControlDark;
                BackColor = SystemColors.Control;
            }
            else
            {
                _grid.EnableHeadersVisualStyles = false;
                _grid.BackgroundColor = p.GridBackground;
                _grid.GridColor = p.GridLines;

                _grid.DefaultCellStyle.BackColor = p.RowEven;
                _grid.DefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.DefaultCellStyle.SelectionBackColor = p.SelectionBackground;
                _grid.DefaultCellStyle.SelectionForeColor = p.SelectionForeground;

                _grid.AlternatingRowsDefaultCellStyle.BackColor = p.RowOdd;
                _grid.AlternatingRowsDefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = p.SelectionBackground;
                _grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = p.SelectionForeground;

                _grid.ColumnHeadersHeight = 36;
                _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = p.HeaderBackground,
                    ForeColor = p.HeaderText,
                    SelectionBackColor = p.HeaderBackground,
                    SelectionForeColor = p.HeaderText,
                    Font = new Font("Segoe UI Semibold", 9.75f),
                    Padding = new Padding(8, 6, 8, 6),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                };
                BackColor = p.ToolbarBackground;
            }

            if (_grid.Rows.Count > 0)
            {
                for (int i = 0; i < _grid.Rows.Count; i++) UpdateRowStyle(i);
                _grid.Invalidate();
            }
        }

        private void UpdateRowStyle(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.RowCount) return;
            var p = ThemePalette.GetPalette(_currentTheme);
            var item = _grid.Rows[rowIndex].DataBoundItem;

            if (item is GroupInfo) return;

            if (rowIndex == _hoveredRowIndex)
            {
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = p.RowHover;
                _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionBackColor = p.SelectionBackground;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = p.SelectionForeground;
            }
            else
            {
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Empty;
                _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Empty;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionBackColor = Color.Empty;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = Color.Empty;
            }
        }

        private void SetupDesignTimeAppearance()
        {
            _grid.Columns.Add("Col1", "Column 1");
            _grid.Columns.Add("Col2", "Column 2");
            _grid.Columns.Add("Col3", "Column 3");
            _grid.Columns[0].Width = 150;
            _grid.Columns[1].Width = 150;
            _grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _grid.Rows.Add("Sample Data 1", "Active", "100");
            _grid.Rows.Add("Sample Data 2", "Inactive", "200");
            _grid.ClearSelection();
        }
    }
}