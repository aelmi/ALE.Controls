using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ALE.Controls;
using ALE.Controls.Theming;

namespace ALE.Controls
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            // Initializes the UI generated in the Designer file
            InitializeComponent();

            // Wire up the PropertyGrid Wrapper
            propertyGrid1.SelectedObject = new GridSettingsWrapper(dataGridEx1, propertyGrid1);

            // Wire up Demo Button Events
            btnDemoGroup.Click += (s, e) => dataGridEx1.AddGroupProperty("Department");
            btnDemoExport.Click += (s, e) => dataGridEx1.ExportToCsv();

            // Wire up Grid Event
            dataGridEx1.ItemDoubleClicked += (s, item) =>
            {
                if (item is Person p) MessageBox.Show($"You double-clicked: {p.FullName}", "Event Fired");
            };

            // Load the Data
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Configure Columns
            dataGridEx1.ConfigureColumns(new[]
            {
                ("Id", "ID", 60),
                ("FullName", "Employee Name", 200),
                ("Department", "Department", 150),
                ("City", "Location", 130),
                ("Salary", "Salary", 120),
                ("IsActive", "Active", 80),
                ("JoinDate", "Join Date", 120)
            });

            // Load Enterprise Data
            var people = new List<Person>
            {
                // Engineering
                new Person { Id = 1, FullName = "Emma Thompson", City = "Brisbane", Salary = 92000m, JoinDate = new DateTime(2021, 5, 12), IsActive = true, Department = "Engineering" },
                new Person { Id = 2, FullName = "James Rodriguez", City = "Sydney", Salary = 78000m, JoinDate = new DateTime(2023, 1, 8), IsActive = true, Department = "Engineering" },
                new Person { Id = 8, FullName = "Lucas Wright", City = "Brisbane", Salary = 125000m, JoinDate = new DateTime(2016, 8, 22), IsActive = true, Department = "Engineering" },
                new Person { Id = 9, FullName = "Mia Taylor", City = "Sydney", Salary = 81000m, JoinDate = new DateTime(2022, 11, 5), IsActive = false, Department = "Engineering" },
                
                // Marketing
                new Person { Id = 13, FullName = "Amelia White", City = "Sydney", Salary = 72000m, JoinDate = new DateTime(2023, 6, 12), IsActive = true, Department = "Marketing" },
                new Person { Id = 14, FullName = "Jack Harris", City = "Brisbane", Salary = 94000m, JoinDate = new DateTime(2020, 4, 25), IsActive = true, Department = "Marketing" },
                new Person { Id = 15, FullName = "Charlotte Martin", City = "Perth", Salary = 76000m, JoinDate = new DateTime(2022, 2, 14), IsActive = false, Department = "Marketing" },
                
                // Finance
                new Person { Id = 3, FullName = "Sophie Chen", City = "Melbourne", Salary = 115000m, JoinDate = new DateTime(2018, 11, 20), IsActive = true, Department = "Finance" },
                new Person { Id = 18, FullName = "Mason Hall", City = "Sydney", Salary = 135000m, JoinDate = new DateTime(2014, 5, 6), IsActive = true, Department = "Finance" },
                
                // Management
                new Person { Id = 6, FullName = "Noah Wilson", City = "Adelaide", Salary = 142000m, JoinDate = new DateTime(2015, 2, 28), IsActive = true, Department = "Management" },
                new Person { Id = 34, FullName = "David Parker", City = "Sydney", Salary = 165000m, JoinDate = new DateTime(2012, 11, 11), IsActive = true, Department = "Management" }
            };

            dataGridEx1.SetData(people);
        }

        // ==========================================
        // DUMMY DATA MODEL
        // ==========================================
        public class Person
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Department { get; set; }
            public string City { get; set; }
            public decimal Salary { get; set; }
            public bool IsActive { get; set; }
            public DateTime JoinDate { get; set; }
        }

        // ==========================================
        // PROPERTY GRID WRAPPER (The Magic Trick)
        // ==========================================
        /// <summary>
        /// This class acts as a proxy. It exposes ONLY the custom properties we want to show 
        /// in the PropertyGrid, hiding the hundreds of default UserControl properties.
        /// </summary>
        public class GridSettingsWrapper
        {
            private readonly DataGridEx _grid;
            private readonly PropertyGrid _pg;

            public GridSettingsWrapper(DataGridEx grid, PropertyGrid pg)
            {
                _grid = grid;
                _pg = pg;
            }

            [Category("1. Appearance")]
            [DisplayName("Grid Theme")]
            [Description("Changes the visual color palette of the entire grid in real-time.")]
            public GridTheme Theme
            {
                get => _grid.Theme;
                set { _grid.Theme = value; _pg.Refresh(); } // Refresh PG in case theme changes elsewhere
            }

            [Category("1. Appearance")]
            [DisplayName("Row Height")]
            [Description("Adjusts the vertical height of data rows. (e.g., 24 for compact, 36 for spacious).")]
            public int RowHeight
            {
                get => _grid.RowHeight;
                set => _grid.RowHeight = value;
            }

            [Category("2. Features")]
            [DisplayName("Show Global Search")]
            [Description("Toggles the visibility of the real-time debounced global search box in the toolbar.")]
            public bool ShowGlobalSearch
            {
                get => _grid.ShowGlobalSearch;
                set => _grid.ShowGlobalSearch = value;
            }

            [Category("2. Features")]
            [DisplayName("Show Theme Selector")]
            [Description("Toggles the visibility of the Theme dropdown menu directly inside the grid's toolbar.")]
            public bool ShowThemeSelector
            {
                get => _grid.ShowThemeSelector;
                set => _grid.ShowThemeSelector = value;
            }

            [Category("2. Features")]
            [DisplayName("Show Context Menu")]
            [Description("Enables or disables the right-click context menu (Copy, Export, Auto-size).")]
            public bool ShowContextMenu
            {
                get => _grid.ShowContextMenu;
                set => _grid.ShowContextMenu = value;
            }
        }
    }
}