using ALE.Controls;
using ALE.Controls.Theming;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ALE.Controls.Testing
{
    public partial class TestForm : Form
    {
        private DataGridEx dataGridEx1;

        public TestForm()
        {
            InitializeComponent();
            SetupGrid();
            LoadSampleData();
        }

        private void SetupGrid()
        {
            dataGridEx1 = new DataGridEx
            {
                Dock = DockStyle.Fill,
                Theme = GridTheme.Light,
            };

            dataGridEx1.GridSelectionChanged += (s, e) =>
            {
                var selected = dataGridEx1.Grid.SelectedRows.Count > 0
                    ? dataGridEx1.Grid.SelectedRows[0].DataBoundItem as Person
                    : null;

                if (selected != null)
                    this.Text = $"Selected: {selected.FullName} - Age {selected.Age}";
            };

            dataGridEx1.ItemDoubleClicked += (s, item) =>
            {
                if (item is Person p)
                    MessageBox.Show($"Double-clicked: {p.FullName}\nEmail: {p.Email}\nCity: {p.City}",
                        "Item Double-Clicked");
            };

            this.Controls.Add(dataGridEx1);
            dataGridEx1.Theme = GridTheme.DarkBlue;
            dataGridEx1.ShowThemeSelector = true;
        }

        private void LoadSampleData()
        {
            var people = new List<Person>
            {
                // --- Engineering ---
                new Person { Id = 1, FullName = "Emma Thompson", Age = 34, Email = "emma.t@example.com", City = "Brisbane", Salary = 92000m, JoinDate = new DateTime(2021, 5, 12), IsActive = true, Department = "Engineering" },
                new Person { Id = 2, FullName = "James Rodriguez", Age = 28, Email = "james.r@company.au", City = "Sydney", Salary = 78000m, JoinDate = new DateTime(2023, 1, 8), IsActive = true, Department = "Engineering" },
                new Person { Id = 8, FullName = "Lucas Wright", Age = 45, Email = "lucas.w@company.au", City = "Brisbane", Salary = 125000m, JoinDate = new DateTime(2016, 8, 22), IsActive = true, Department = "Engineering" },
                new Person { Id = 9, FullName = "Mia Taylor", Age = 29, Email = "mia.t@company.au", City = "Sydney", Salary = 81000m, JoinDate = new DateTime(2022, 11, 5), IsActive = false, Department = "Engineering" },
                new Person { Id = 10, FullName = "Ethan Anderson", Age = 38, Email = "ethan.a@company.au", City = "Melbourne", Salary = 105000m, JoinDate = new DateTime(2019, 3, 14), IsActive = true, Department = "Engineering" },
                new Person { Id = 11, FullName = "Harper Thomas", Age = 31, Email = "harper.t@company.au", City = "Brisbane", Salary = 89000m, JoinDate = new DateTime(2021, 9, 30), IsActive = true, Department = "Engineering" },
                new Person { Id = 12, FullName = "Oliver Jackson", Age = 42, Email = "oliver.j@company.au", City = "Melbourne", Salary = 118000m, JoinDate = new DateTime(2017, 1, 18), IsActive = false, Department = "Engineering" },

                // --- Marketing ---
                new Person { Id = 13, FullName = "Amelia White", Age = 26, Email = "amelia.w@example.com", City = "Sydney", Salary = 72000m, JoinDate = new DateTime(2023, 6, 12), IsActive = true, Department = "Marketing" },
                new Person { Id = 14, FullName = "Jack Harris", Age = 35, Email = "jack.h@example.com", City = "Brisbane", Salary = 94000m, JoinDate = new DateTime(2020, 4, 25), IsActive = true, Department = "Marketing" },
                new Person { Id = 15, FullName = "Charlotte Martin", Age = 28, Email = "charlotte.m@example.com", City = "Perth", Salary = 76000m, JoinDate = new DateTime(2022, 2, 14), IsActive = false, Department = "Marketing" },
                new Person { Id = 16, FullName = "William Lee", Age = 41, Email = "william.l@example.com", City = "Sydney", Salary = 112000m, JoinDate = new DateTime(2018, 10, 8), IsActive = true, Department = "Marketing" },
                new Person { Id = 17, FullName = "Isabella Walker", Age = 33, Email = "isabella.w@example.com", City = "Brisbane", Salary = 88000m, JoinDate = new DateTime(2021, 7, 19), IsActive = true, Department = "Marketing" },

                // --- Finance ---
                new Person { Id = 3, FullName = "Sophie Chen", Age = 41, Email = "s.chen@workmail.com", City = "Melbourne", Salary = 115000m, JoinDate = new DateTime(2018, 11, 20), IsActive = true, Department = "Finance" },
                new Person { Id = 18, FullName = "Mason Hall", Age = 50, Email = "mason.h@workmail.com", City = "Sydney", Salary = 135000m, JoinDate = new DateTime(2014, 5, 6), IsActive = true, Department = "Finance" },
                new Person { Id = 19, FullName = "Evelyn Allen", Age = 36, Email = "evelyn.a@workmail.com", City = "Melbourne", Salary = 98000m, JoinDate = new DateTime(2020, 1, 22), IsActive = true, Department = "Finance" },
                new Person { Id = 20, FullName = "Elijah Young", Age = 29, Email = "elijah.y@workmail.com", City = "Brisbane", Salary = 82000m, JoinDate = new DateTime(2022, 8, 11), IsActive = false, Department = "Finance" },
                new Person { Id = 21, FullName = "Abigail King", Age = 44, Email = "abigail.k@workmail.com", City = "Sydney", Salary = 121000m, JoinDate = new DateTime(2017, 12, 3), IsActive = true, Department = "Finance" },

                // --- Sales ---
                new Person { Id = 4, FullName = "Liam Patel", Age = 25, Email = "liam.p@startup.au", City = "Brisbane", Salary = 65000m, JoinDate = new DateTime(2024, 3, 15), IsActive = false, Department = "Sales" },
                new Person { Id = 22, FullName = "Benjamin Scott", Age = 32, Email = "ben.s@startup.au", City = "Sydney", Salary = 85000m, JoinDate = new DateTime(2021, 10, 28), IsActive = true, Department = "Sales" },
                new Person { Id = 23, FullName = "Emily Green", Age = 27, Email = "emily.g@startup.au", City = "Melbourne", Salary = 71000m, JoinDate = new DateTime(2023, 4, 16), IsActive = true, Department = "Sales" },
                new Person { Id = 24, FullName = "James Baker", Age = 39, Email = "james.b@startup.au", City = "Brisbane", Salary = 105000m, JoinDate = new DateTime(2019, 6, 9), IsActive = true, Department = "Sales" },
                new Person { Id = 25, FullName = "Elizabeth Adams", Age = 30, Email = "liz.a@startup.au", City = "Perth", Salary = 79000m, JoinDate = new DateTime(2022, 1, 25), IsActive = false, Department = "Sales" },
                new Person { Id = 26, FullName = "Alexander Nelson", Age = 46, Email = "alex.n@startup.au", City = "Sydney", Salary = 128000m, JoinDate = new DateTime(2015, 9, 14), IsActive = true, Department = "Sales" },

                // --- HR ---
                new Person { Id = 5, FullName = "Olivia Nguyen", Age = 37, Email = "olivia.n@corp.com", City = "Perth", Salary = 98000m, JoinDate = new DateTime(2020, 7, 1), IsActive = true, Department = "HR" },
                new Person { Id = 27, FullName = "Michael Carter", Age = 42, Email = "michael.c@corp.com", City = "Brisbane", Salary = 108000m, JoinDate = new DateTime(2018, 3, 5), IsActive = true, Department = "HR" },
                new Person { Id = 28, FullName = "Sofia Mitchell", Age = 31, Email = "sofia.m@corp.com", City = "Melbourne", Salary = 84000m, JoinDate = new DateTime(2021, 11, 18), IsActive = false, Department = "HR" },
                new Person { Id = 29, FullName = "Daniel Perez", Age = 35, Email = "daniel.p@corp.com", City = "Sydney", Salary = 92000m, JoinDate = new DateTime(2019, 8, 27), IsActive = true, Department = "HR" },

                // --- IT Support ---
                new Person { Id = 30, FullName = "Matthew Roberts", Age = 28, Email = "matt.r@tech.au", City = "Brisbane", Salary = 74000m, JoinDate = new DateTime(2023, 2, 9), IsActive = true, Department = "IT Support" },
                new Person { Id = 31, FullName = "Avery Turner", Age = 24, Email = "avery.t@tech.au", City = "Sydney", Salary = 62000m, JoinDate = new DateTime(2024, 1, 15), IsActive = true, Department = "IT Support" },
                new Person { Id = 32, FullName = "Joseph Phillips", Age = 33, Email = "joe.p@tech.au", City = "Brisbane", Salary = 86000m, JoinDate = new DateTime(2020, 10, 4), IsActive = false, Department = "IT Support" },
                new Person { Id = 33, FullName = "Chloe Campbell", Age = 29, Email = "chloe.c@tech.au", City = "Melbourne", Salary = 78000m, JoinDate = new DateTime(2022, 5, 21), IsActive = true, Department = "IT Support" },
    
                // --- Management ---
                new Person { Id = 6, FullName = "Noah Wilson", Age = 52, Email = "noah.w@enterprise.au", City = "Adelaide", Salary = 142000m, JoinDate = new DateTime(2015, 2, 28), IsActive = true, Department = "Management" },
                new Person { Id = 34, FullName = "David Parker", Age = 55, Email = "david.p@enterprise.au", City = "Sydney", Salary = 165000m, JoinDate = new DateTime(2012, 11, 11), IsActive = true, Department = "Management" },
                new Person { Id = 35, FullName = "Grace Evans", Age = 48, Email = "grace.e@enterprise.au", City = "Brisbane", Salary = 152000m, JoinDate = new DateTime(2016, 4, 30), IsActive = true, Department = "Management" },
    
                // --- Design ---
                new Person { Id = 7, FullName = "Ava Martinez", Age = 31, Email = "ava.m@gmailpro.com", City = "Gold Coast", Salary = 84000m, JoinDate = new DateTime(2022, 9, 10), IsActive = false, Department = "Design" },
                new Person { Id = 36, FullName = "Samuel Edwards", Age = 27, Email = "sam.e@gmailpro.com", City = "Brisbane", Salary = 73000m, JoinDate = new DateTime(2023, 7, 2), IsActive = true, Department = "Design" },
                new Person { Id = 37, FullName = "Victoria Collins", Age = 34, Email = "vic.c@gmailpro.com", City = "Gold Coast", Salary = 91000m, JoinDate = new DateTime(2020, 12, 15), IsActive = true, Department = "Design" }
            };

            dataGridEx1.ConfigureColumns(new[]
            {
                ("Id",        "ID",          60),
                ("FullName",  "Name",       180),
                ("Age",       "Age",         70),
                ("Email",     "Email",      220),
                ("City",      "City",       140),
                ("Salary",    "Salary",     110),
                ("JoinDate",  "Join Date",  120),
                ("IsActive",  "Active",      80),
                ("Department","Department", 140)
            });

            dataGridEx1.SetData(people);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public string Department { get; set; }
    }
}