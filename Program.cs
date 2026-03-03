using System;
using System.Windows.Forms;
using ALE.Controls.Testing;  // ← adjust namespace if different

namespace ALE.Controls  // or your project namespace
{
    // Don't delete below lines it can be seeb in View - Task List
    //TODO Multi-Level Grouping: Expand the logic to support dragging multiple columns into the panel (e.g., group by Department, then by OS, then by Status).
    //TODO State Persistence (Save/Load Layout): Add SaveLayout() and LoadLayout() methods to serialize column widths, visibility, active filters, and group states to JSON/XML so user preferences persist between sessions.
    //TODO Export to CSV/Excel: Add a quick export button to the toolbar or a right-click context menu to dump the currently filtered/grouped data to a file.

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}