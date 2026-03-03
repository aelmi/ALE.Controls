using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ALE.Controls.Filtering;
using ALE.Controls.Theming;

namespace ALE.Controls
{
    public partial class DataGridEx : UserControl
    {
        private DataGridView _grid;
        private List<object> _sourceData = new();
        private Type _itemType;
        private BindingList<object> _bindingList;
        private FilterGroup _activeFilter;

        private ToolStrip _toolStrip;
        private ToolStripDropDownButton _btnTheme;
        private ToolStripLabel _lblFilterStatus;
        private ToolStripButton _btnClearFilter;
        private ToolStripButton _btnAdvancedFilter;

        private GridTheme _currentTheme = GridTheme.None;
        private bool _showThemeSelector = false;
        private int _hoveredRowIndex = -1; // Track the hovered row

        private readonly Dictionary<string, string> _propertyToHeader = new(StringComparer.OrdinalIgnoreCase);

        public event EventHandler GridSelectionChanged;
        public event EventHandler<object> ItemDoubleClicked;

        [Browsable(false)]
        public DataGridView Grid => _grid;

        [Category("Appearance")]
        public GridTheme Theme
        {
            get => _currentTheme;
            set
            {
                _currentTheme = value;
                ApplyTheme();
            }
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

        public DataGridEx()
        {
            InitializeComponent();
            SetupGrid();
            SetupToolbarAndTheme();
            ApplyTheme();
        }

        private void SetupGrid()
        {
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                MultiSelect = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
            };

            _grid.SelectionChanged += (_, _) => GridSelectionChanged?.Invoke(this, EventArgs.Empty);
            _grid.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0)
                    ItemDoubleClicked?.Invoke(this, _grid.Rows[e.RowIndex].DataBoundItem);
            };

            // FEATURE: Hover implementation
            _grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex != _hoveredRowIndex)
                {
                    int oldIndex = _hoveredRowIndex;
                    _hoveredRowIndex = e.RowIndex;
                    UpdateRowStyle(oldIndex);
                    UpdateRowStyle(_hoveredRowIndex);
                }
            };

            _grid.CellMouseLeave += (s, e) =>
            {
                if (_hoveredRowIndex >= 0)
                {
                    int oldIndex = _hoveredRowIndex;
                    _hoveredRowIndex = -1;
                    UpdateRowStyle(oldIndex);
                }
            };

            _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;

            Controls.Add(_grid);
        }

        private void UpdateRowStyle(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.RowCount) return;

            var p = ThemePalette.GetPalette(_currentTheme);
            var row = _grid.Rows[rowIndex];

            if (rowIndex == _hoveredRowIndex)
            {
                row.DefaultCellStyle.BackColor = p.RowHover;
            }
            else
            {
                // Reset to standard alternating/even colors
                row.DefaultCellStyle.BackColor = (rowIndex % 2 == 0) ? p.RowEven : p.RowOdd;
            }
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
                var item = new ToolStripMenuItem(t.ToString(), null, (_, _) => Theme = t);
                _btnTheme.DropDownItems.Add(item);
            }

            _btnTheme.Visible = _showThemeSelector;

            _toolStrip.Items.AddRange(new ToolStripItem[]
            {
                _btnAdvancedFilter,
                _btnClearFilter,
                new ToolStripSeparator(),
                _lblFilterStatus,
                new ToolStripControlHost(new Label { Width = 20 }),
                _btnTheme
            });
            topPanel.Controls.Add(_toolStrip);
            Controls.Add(topPanel);
        }

        private void ApplyTheme()
        {
            var p = ThemePalette.GetPalette(_currentTheme);

            if (_currentTheme == GridTheme.None)
            {
                _grid.EnableHeadersVisualStyles = true;
                return;
            }

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

        public void SetData<T>(IEnumerable<T> data) where T : class
        {
            _itemType = typeof(T);
            _sourceData = data?.Cast<object>().ToList() ?? new List<object>();
            _bindingList = new BindingList<object>(_sourceData);
            _grid.DataSource = _bindingList;
            PopulateAllProperties();
            ApplySortAndFilter();
        }

        public void ConfigureColumns(IEnumerable<(string Property, string Header, int Width)> columns)
        {
            _grid.Columns.Clear();
            _propertyToHeader.Clear();
            foreach (var (prop, header, w) in columns)
            {
                _grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = prop,
                    HeaderText = header,
                    Width = w,
                    SortMode = DataGridViewColumnSortMode.Programmatic,
                    Name = prop
                });
                _propertyToHeader[prop] = header;
            }
            PopulateAllProperties();
        }

        private void PopulateAllProperties()
        {
            if (_itemType == null) return;
            foreach (var prop in _itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!_propertyToHeader.ContainsKey(prop.Name)) _propertyToHeader[prop.Name] = prop.Name;
            }
        }

        private string _sortProperty;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var col = _grid.Columns[e.ColumnIndex];
            if (string.IsNullOrEmpty(col.DataPropertyName)) return;

            if (_sortProperty == col.DataPropertyName)
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else
            {
                _sortProperty = col.DataPropertyName;
                _sortDirection = ListSortDirection.Ascending;
            }
            ApplySortAndFilter();
            UpdateSortGlyphs();
        }

        private void UpdateSortGlyphs()
        {
            foreach (DataGridViewColumn c in _grid.Columns) c.HeaderCell.SortGlyphDirection = SortOrder.None;
            if (!string.IsNullOrEmpty(_sortProperty) && _grid.Columns.Contains(_sortProperty))
            {
                var col = _grid.Columns[_sortProperty];
                col.HeaderCell.SortGlyphDirection = _sortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        private void ApplySortAndFilter()
        {
            if (_sourceData == null || _sourceData.Count == 0) return;
            var filtered = _sourceData.AsEnumerable();

            if (_activeFilter != null && !_activeFilter.IsEmpty)
                filtered = filtered.Where(item => _activeFilter.Evaluate(item, _itemType));

            if (!string.IsNullOrEmpty(_sortProperty))
            {
                var prop = _itemType.GetProperty(_sortProperty);
                if (prop != null)
                {
                    filtered = _sortDirection == ListSortDirection.Ascending
                        ? filtered.OrderBy(x => prop.GetValue(x) ?? DBNull.Value)
                        : filtered.OrderByDescending(x => prop.GetValue(x) ?? DBNull.Value);
                }
            }

            _bindingList = new BindingList<object>(filtered.ToList());
            _grid.DataSource = _bindingList;
            UpdateFilterStatus();
        }

        private void ShowFilterBuilder()
        {
            if (_itemType == null) return;
            var allFields = _propertyToHeader.Select(kvp => (Property: kvp.Key, Header: kvp.Value)).OrderBy(x => x.Header).ToList();
            using var dlg = new FilterExpressionDialog(allFields, _itemType, _activeFilter, _currentTheme);
            if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
            {
                _activeFilter = dlg.ResultFilter;
                ApplySortAndFilter();
            }
        }

        private void ClearFilter()
        {
            _activeFilter = null;
            ApplySortAndFilter();
        }

        private void UpdateFilterStatus()
        {
            if (_activeFilter == null || _activeFilter.IsEmpty)
            {
                _lblFilterStatus.Text = "No filter";
                _btnClearFilter.Visible = false;
            }
            else
            {
                _lblFilterStatus.Text = $"Filtered: {_bindingList.Count} / {_sourceData.Count}";
                _btnClearFilter.Visible = true;
            }
        }
    }
}