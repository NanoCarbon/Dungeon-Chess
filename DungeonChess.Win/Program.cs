using System;
using System.Windows.Forms;
using DungeonChess.Win;

namespace DungeonChess
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DungeonChessApplicationContext context = new DungeonChessApplicationContext();
            Application.Run(context);
        }
    }
}
