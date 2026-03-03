using System;
using System.Windows.Forms;
using ALE.Controls.Testing;  // ← adjust namespace if different

namespace ALE.Controls  // or your project namespace
{
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