using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ALE.Controls.Theming;

namespace ALE.Controls
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            // Initializes the UI generated in the Designer file
            InitializeComponent();

            // Wire up the PropertyGrid Wrapper (Exposes our custom properties)
            propertyGrid1.SelectedObject = new GridSettingsWrapper(dataGridEx1, propertyGrid1);

            // Wire up Demo Button Events
            btnDemoGroup.Click += (s, e) => dataGridEx1.AddGroupProperty("Department");
            btnDemoExport.Click += (s, e) => dataGridEx1.ExportToCsv();

            // Wire up Grid Double-Click Event
            dataGridEx1.ItemDoubleClicked += (s, item) =>
            {
                if (item is Person p) MessageBox.Show($"You double-clicked: {p.FullName}\nDepartment: {p.Department}", "Event Fired");
            };

            // Load the Data (50 Generated Rows)
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Configure Columns
            dataGridEx1.ConfigureColumns(new[]
            {
                ("Id", "ID", 60),
                ("FullName", "Employee Name", 200),
                ("Department", "Department", 160),
                ("City", "Location", 130),
                ("Salary", "Salary", 120),
                ("IsActive", "Active", 80),
                ("JoinDate", "Join Date", 120)
            });

            // Auto-Generate 50 realistic records for testing
            var firstNames = new[] { "Emma", "James", "Lucas", "Mia", "Ethan", "Harper", "Oliver", "Amelia", "Jack", "Charlotte", "William", "Isabella", "Sophie", "Mason", "Evelyn", "Elijah", "Abigail", "Liam", "Benjamin", "Emily", "Alexander", "Olivia" };
            var lastNames = new[] { "Thompson", "Rodriguez", "Wright", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Lee", "Walker", "Chen", "Hall", "Allen", "Young", "King", "Patel", "Scott", "Baker", "Adams" };
            var depts = new[] { "Engineering", "Marketing", "Finance", "Management", "HR", "Sales", "IT Support", "Legal" };
            var cities = new[] { "Brisbane", "Sydney", "Melbourne", "Perth", "Adelaide", "Hobart", "Gold Coast" };

            var people = new List<Person>();

            // Using a fixed seed (42) so the random data is consistent every time you run the app
            var rnd = new Random(42);

            for (int i = 1; i <= 50; i++)
            {
                string fName = firstNames[rnd.Next(firstNames.Length)];
                string lName = lastNames[rnd.Next(lastNames.Length)];

                people.Add(new Person
                {
                    Id = i,
                    FullName = $"{fName} {lName}",
                    Department = depts[rnd.Next(depts.Length)],
                    City = cities[rnd.Next(cities.Length)],
                    Salary = rnd.Next(60, 160) * 1000m,
                    IsActive = rnd.Next(100) > 20, // 80% chance to be active
                    JoinDate = new DateTime(rnd.Next(2015, 2024), rnd.Next(1, 13), rnd.Next(1, 28))
                });
            }

            // Bind the 50 rows to the grid
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

            [Category("1. Appearance")]
            [DisplayName("Search Box Width")]
            [Description("Adjusts the width of the global search box in real-time.")]
            public int SearchBoxWidth
            {
                get => _grid.SearchBoxWidth;
                set => _grid.SearchBoxWidth = value;
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
            [DisplayName("Show Column Selector")]
            [Description("Toggles the visibility of the Columns dropdown menu in the toolbar.")]
            public bool ShowColumnSelector
            {
                get => _grid.ShowColumnSelector;
                set => _grid.ShowColumnSelector = value;
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