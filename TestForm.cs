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
                new Person { Id = 1, FullName = "Emma Thompson", Age = 34, Email = "emma.t@example.com", City = "Brisbane", Salary = 92000m, JoinDate = new DateTime(2021, 5, 12), IsActive = true, Department = "Marketing" },
                new Person { Id = 2, FullName = "James Rodriguez", Age = 28, Email = "james.r@company.au", City = "Sydney", Salary = 78000m, JoinDate = new DateTime(2023, 1, 8), IsActive = true, Department = "Engineering" },
                new Person { Id = 3, FullName = "Sophie Chen", Age = 41, Email = "s.chen@workmail.com", City = "Melbourne", Salary = 115000m, JoinDate = new DateTime(2018, 11, 20), IsActive = true, Department = "Finance" },
                new Person { Id = 4, FullName = "Liam Patel", Age = 25, Email = "liam.p@startup.au", City = "Brisbane", Salary = 65000m, JoinDate = new DateTime(2024, 3, 15), IsActive = false, Department = "Sales" },
                new Person { Id = 5, FullName = "Olivia Nguyen", Age = 37, Email = "olivia.n@corp.com", City = "Perth", Salary = 98000m, JoinDate = new DateTime(2020, 7, 1), IsActive = true, Department = "HR" },
                new Person { Id = 6, FullName = "Noah Wilson", Age = 52, Email = "noah.w@enterprise.au", City = "Adelaide", Salary = 142000m, JoinDate = new DateTime(2015, 2, 28), IsActive = true, Department = "Management" },
                new Person { Id = 7, FullName = "Ava Martinez", Age = 31, Email = "ava.m@gmailpro.com", City = "Gold Coast", Salary = 84000m, JoinDate = new DateTime(2022, 9, 10), IsActive = false, Department = "Design" },
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