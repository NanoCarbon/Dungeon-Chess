using System;               // ✅ This is required
using System.Windows.Forms;
using DungeonChess.Core;

namespace DungeonChess.Win
{
    static class Program
    {
        [STAThread]          // This attribute ensures proper threading for WinForms
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
