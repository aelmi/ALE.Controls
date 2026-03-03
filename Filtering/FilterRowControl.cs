using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ALE.Controls.Theming;

namespace ALE.Controls.Filtering
{
    public class FilterRowControl : UserControl
    {
        private ComboBox _cboColumn;
        private ComboBox _cboOperator;
        private TextBox _txtValue;
        private Button _btnRemove;
        private List<(string Property, string Header)> _columns;
        private Type _dataSourceType;
        private readonly GridTheme _theme;
        private readonly ThemeColors _colors;

        public event EventHandler RemoveClicked;
        public event EventHandler ConditionChanged;

        public FilterCondition Condition { get; private set; }

        // --- DESIGN TIME SUPPORT ---
        // Default constructor allows Visual Studio to render the control
        public FilterRowControl()
            : this(
                new List<(string, string)> { ("SampleField", "Sample Field"), ("Age", "Age") },
                typeof(DesignTimeDummy),
                null,
                GridTheme.Light)
        {
        }

        public FilterRowControl(
            List<(string Property, string Header)> columns,
            Type dataSourceType,
            FilterCondition existingCondition = null,
            GridTheme theme = GridTheme.Light)
        {
            _columns = columns;
            _dataSourceType = dataSourceType;
            _theme = theme;
            _colors = ThemePalette.GetPalette(theme);

            Condition = existingCondition ?? new FilterCondition
            {
                PropertyName = columns.FirstOrDefault().Property,
                Operator = FilterOperator.Contains,
                Value = ""
            };

            Height = 36;
            Dock = DockStyle.Top;
            Margin = new Padding(0, 2, 0, 2);
            BackColor = Color.Transparent;

            BuildUI();
            WireEvents();
        }

        private void BuildUI()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));

            _cboColumn = CreateStyledComboBox();
            foreach (var col in _columns)
                _cboColumn.Items.Add(col.Header);

            int colIndex = _columns.FindIndex(c => c.Property == Condition.PropertyName);
            _cboColumn.SelectedIndex = colIndex >= 0 ? colIndex : 0;

            _cboOperator = CreateStyledComboBox();

            _txtValue = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                Text = Condition.Value ?? "",
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2, 4, 2, 4),
                BackColor = _colors.RowEven,
                ForeColor = _colors.TextPrimary
            };

            _btnRemove = new Button
            {
                Text = "×",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14f),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 4, 0, 4),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(180, 60, 60)
            };
            _btnRemove.FlatAppearance.BorderSize = 0;

            layout.Controls.Add(_cboColumn, 0, 0);
            layout.Controls.Add(_cboOperator, 1, 0);
            layout.Controls.Add(_txtValue, 2, 0);
            layout.Controls.Add(_btnRemove, 3, 0);

            Controls.Add(layout);
            RefreshOperators();
        }

        private ComboBox CreateStyledComboBox()
        {
            var cbo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(2, 4, 2, 4),
                ItemHeight = 24,
                DrawMode = DrawMode.OwnerDrawFixed,
                BackColor = _colors.RowEven,
                ForeColor = _colors.TextPrimary
            };

            cbo.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;

                var combo = (ComboBox)s;
                var text = combo.GetItemText(combo.Items[e.Index]);

                e.DrawBackground();

                TextRenderer.DrawText(e.Graphics, text, e.Font,
                    e.Bounds, _colors.TextPrimary, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                e.DrawFocusRectangle();
            };

            return cbo;
        }

        private void WireEvents()
        {
            _cboColumn.SelectedIndexChanged += (s, e) =>
            {
                if (_cboColumn.SelectedIndex >= 0 && _cboColumn.SelectedIndex < _columns.Count)
                {
                    Condition.PropertyName = _columns[_cboColumn.SelectedIndex].Property;
                    RefreshOperators();
                    ConditionChanged?.Invoke(this, EventArgs.Empty);
                }
            };

            _cboOperator.SelectedIndexChanged += (s, e) =>
            {
                if (_cboOperator.SelectedItem is OperatorInfo opInfo)
                {
                    Condition.Operator = opInfo.Operator;
                    UpdateValueVisibility();
                    ConditionChanged?.Invoke(this, EventArgs.Empty);
                }
            };

            _txtValue.TextChanged += (s, e) =>
            {
                Condition.Value = _txtValue.Text;
                ConditionChanged?.Invoke(this, EventArgs.Empty);
            };

            _btnRemove.Click += (s, e) => RemoveClicked?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshOperators()
        {
            if (_cboColumn.SelectedIndex < 0) return;

            string propName = _columns[_cboColumn.SelectedIndex].Property;
            var propInfo = _dataSourceType.GetProperty(propName);
            Type colType = propInfo?.PropertyType ?? typeof(string);

            var nullableUnderlyingType = Nullable.GetUnderlyingType(colType);
            if (nullableUnderlyingType != null)
                colType = nullableUnderlyingType;

            _cboOperator.Items.Clear();

            IEnumerable<FilterOperator> opsToShow;

            if (colType == typeof(string))
            {
                opsToShow = new[] {
                    FilterOperator.Equals, FilterOperator.NotEquals,
                    FilterOperator.Contains, FilterOperator.NotContains,
                    FilterOperator.StartsWith, FilterOperator.EndsWith,
                    FilterOperator.Like, FilterOperator.NotLike,
                    FilterOperator.IsNull, FilterOperator.IsNotNull
                };
            }
            else if (IsNumericType(colType) || colType == typeof(DateTime))
            {
                opsToShow = new[] {
                    FilterOperator.Equals, FilterOperator.NotEquals,
                    FilterOperator.GreaterThan, FilterOperator.GreaterThanOrEqual,
                    FilterOperator.LessThan, FilterOperator.LessThanOrEqual,
                    FilterOperator.Between, FilterOperator.NotBetween,
                    FilterOperator.IsNull, FilterOperator.IsNotNull
                };
            }
            else
            {
                opsToShow = new[] { FilterOperator.Equals, FilterOperator.NotEquals, FilterOperator.IsNull, FilterOperator.IsNotNull };
            }

            foreach (var op in opsToShow)
            {
                _cboOperator.Items.Add(new OperatorInfo(op, FilterCondition.OperatorToString(op)));
            }

            bool found = false;
            for (int i = 0; i < _cboOperator.Items.Count; i++)
            {
                if (((OperatorInfo)_cboOperator.Items[i]).Operator == Condition.Operator)
                {
                    _cboOperator.SelectedIndex = i;
                    found = true;
                    break;
                }
            }

            if (!found && _cboOperator.Items.Count > 0)
            {
                _cboOperator.SelectedIndex = 0;
                Condition.Operator = ((OperatorInfo)_cboOperator.Items[0]).Operator;
            }

            UpdateValueVisibility();
        }

        private bool IsNumericType(Type t)
        {
            return t == typeof(int) || t == typeof(double) || t == typeof(float)
                || t == typeof(decimal) || t == typeof(long) || t == typeof(short)
                || t == typeof(byte) || t == typeof(uint) || t == typeof(ulong)
                || t == typeof(ushort) || t == typeof(sbyte);
        }

        private void UpdateValueVisibility()
        {
            bool isUnary = Condition.Operator == FilterOperator.IsNull ||
                          Condition.Operator == FilterOperator.IsNotNull;
            _txtValue.Visible = !isUnary;
        }

        // Helper class for Design Time dummy data
        private class DesignTimeDummy { public string SampleField { get; set; } public int Age { get; set; } }

        private class OperatorInfo
        {
            public FilterOperator Operator { get; }
            public string Display { get; }

            public OperatorInfo(FilterOperator op, string display)
            {
                Operator = op;
                Display = display;
            }

            public override string ToString() => Display;
        }
    }
}