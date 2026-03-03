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
                if (!_propertyToHeader.ContainsKey(prop.Name)) _propertyToHeader[prop.Name] = prop.Name;
            }
        }

        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.Button != MouseButtons.Left) return;

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

            List<object> finalDisplayList = new();

            if (_groupProperties.Count > 0)
            {
                // Trigger recursive grouping
                finalDisplayList.AddRange(GenerateGroupedList(filtered, 0, ""));
            }
            else
            {
                finalDisplayList = filtered.ToList();
            }

            _bindingList = new GroupedBindingList(finalDisplayList, _itemType);
            _grid.DataSource = _bindingList;
            UpdateFilterStatus();
        }

        // ==========================================
        // RECURSIVE MULTI-LEVEL ENGINE
        // ==========================================
        private IEnumerable<object> GenerateGroupedList(IEnumerable<object> data, int level, string parentPath)
        {
            // Base Case: We've reached the deepest group layer, just return the raw data rows
            if (level >= _groupProperties.Count)
            {
                foreach (var item in data) yield return item;
                yield break;
            }

            string currentPropName = _groupProperties[level];
            var propInfo = _itemType.GetProperty(currentPropName);
            if (propInfo == null) yield break;

            var groups = data.GroupBy(x => propInfo.GetValue(x) ?? "None");

            foreach (var g in groups)
            {
                string groupKey = g.Key.ToString();

                // Track composite path to avoid collapse-state collisions between identically named sub-groups
                string currentPath = string.IsNullOrEmpty(parentPath) ? groupKey : $"{parentPath}|{groupKey}";
                bool isCollapsed = _collapsedGroups.GetValueOrDefault(currentPath, false);

                yield return new GroupInfo
                {
                    GroupValue = g.Key,
                    PropertyName = _propertyToHeader.ContainsKey(currentPropName) ? _propertyToHeader[currentPropName] : currentPropName,
                    ChildCount = g.Count(), // Shows how many actual data rows exist inside this layer
                    IsCollapsed = isCollapsed,
                    Level = level,
                    GroupPath = currentPath
                };

                // Drill down to the next grouping layer if this node is open
                if (!isCollapsed)
                {
                    foreach (var child in GenerateGroupedList(g, level + 1, currentPath))
                    {
                        yield return child;
                    }
                }
            }
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
                int filteredCount = _bindingList.Count(x => !(x is GroupInfo));
                _lblFilterStatus.Text = $"Filtered: {filteredCount} / {_sourceData.Count}";
                _btnClearFilter.Visible = true;
            }
        }
    }
}