using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ALE.Controls.Filtering;
using ALE.Controls.Grouping;

namespace ALE.Controls
{
    public partial class DataGridEx
    {
        public void SetData<T>(IEnumerable<T> data) where T : class
        {
            _itemType = typeof(T);
            _sourceData = data?.Cast<object>().ToList() ?? new List<object>();
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
            foreach (var prop in _itemType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (!_propertyToHeader.ContainsKey(prop.Name))
                {
                    _propertyToHeader[prop.Name] = prop.Name;
                }
            }
        }

        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Do not sort if it's a right click (handled by context menu)
            if (e.ColumnIndex < 0 || e.Button != MouseButtons.Left) return;

            var col = _grid.Columns[e.ColumnIndex];
            if (string.IsNullOrEmpty(col.DataPropertyName)) return;

            if (_sortProperty == col.DataPropertyName)
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
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
            foreach (DataGridViewColumn c in _grid.Columns)
            {
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

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
            {
                filtered = filtered.Where(item => _activeFilter.Evaluate(item, _itemType));
            }

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

            List<object> finalDisplayList = new();

            if (!string.IsNullOrEmpty(_groupProperty))
            {
                var groupProp = _itemType.GetProperty(_groupProperty);
                if (groupProp != null)
                {
                    var groups = filtered.GroupBy(x => groupProp.GetValue(x) ?? "None");

                    foreach (var g in groups)
                    {
                        string groupKey = g.Key.ToString();
                        bool isCollapsed = _collapsedGroups.GetValueOrDefault(groupKey, false);

                        finalDisplayList.Add(new GroupInfo
                        {
                            GroupValue = g.Key,
                            PropertyName = _propertyToHeader.ContainsKey(_groupProperty) ? _propertyToHeader[_groupProperty] : _groupProperty,
                            ChildCount = g.Count(),
                            IsCollapsed = isCollapsed
                        });

                        if (!isCollapsed)
                        {
                            finalDisplayList.AddRange(g);
                        }
                    }
                }
            }
            else
            {
                finalDisplayList = filtered.ToList();
            }

            // Bind using our safe ITypedList wrapper to prevent schema corruption
            _bindingList = new GroupedBindingList(finalDisplayList, _itemType);
            _grid.DataSource = _bindingList;
            UpdateFilterStatus();
        }

        private void ShowFilterBuilder()
        {
            if (_itemType == null) return;

            var allFields = _propertyToHeader
                .Select(kvp => (Property: kvp.Key, Header: kvp.Value))
                .OrderBy(x => x.Header)
                .ToList();

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
                int filteredCount = _bindingList.Count(x => !(x is GroupInfo));
                _lblFilterStatus.Text = $"Filtered: {filteredCount} / {_sourceData.Count}";
                _btnClearFilter.Visible = true;
            }
        }
    }
}