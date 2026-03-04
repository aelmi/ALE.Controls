using System;
using System.Windows.Forms;

namespace ALE.Controls
{
    //TODO State Persistence (Save/Load Layout): Add SaveLayout() and LoadLayout() methods to serialize column widths, visibility, active filters, and group states to JSON/XML so user preferences persist between sessions.
    //TODO Export to Excel

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