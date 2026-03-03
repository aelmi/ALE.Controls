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
        // ALL PRIVATE FIELDS DECLARATION
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

        private Panel _groupPanel;
        private Label _lblGroupHint;
        private FlowLayoutPanel _groupChipsPanel;
        private Rectangle _dragBoxFromMouseDown;
        private string _draggedColumnName;
        private string _groupProperty = null;
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

        [Browsable(false)]
        public IReadOnlyDictionary<string, string> PropertyHeaders => _propertyToHeader;

        // ==========================================
        // INITIALIZATION
        // ==========================================
        public DataGridEx()
        {
            InitializeComponent();

            SetupToolbarAndTheme();
            SetupGroupPanel();
            SetupGrid();

            ApplyTheme();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                SetupDesignTimeAppearance();
            }
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
            {
                _btnTheme.DropDownItems.Add(new ToolStripMenuItem(t.ToString(), null, (_, _) => Theme = t));
            }

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

            // SCRUB EXISTING ROWS: Force existing rows to inherit the new theme immediately
            if (_grid.Rows.Count > 0)
            {
                for (int i = 0; i < _grid.Rows.Count; i++)
                {
                    UpdateRowStyle(i);
                }
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
                // Apply the hover overlay directly
                _grid.Rows[rowIndex].DefaultCellStyle.BackColor = p.RowHover;
                _grid.Rows[rowIndex].DefaultCellStyle.ForeColor = p.TextPrimary;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionBackColor = p.SelectionBackground;
                _grid.Rows[rowIndex].DefaultCellStyle.SelectionForeColor = p.SelectionForeground;
            }
            else
            {
                // FIX: Setting it to Color.Empty strips away the old memory, 
                // forcing the row to inherit the pristine Grid-Level theme you just applied.
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