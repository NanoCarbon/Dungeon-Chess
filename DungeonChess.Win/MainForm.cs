using System;
using System.Drawing;
using System.Windows.Forms;

namespace DungeonChess.Win
{
    public class MainForm : Form
    {
        private const int TileSize = 40;
        private const int BoardSize = 8;

        public MainForm()
        {
            this.Text = "Dungeon Chess Board";
            this.ClientSize = new Size(TileSize * BoardSize, TileSize * BoardSize);
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Font font = new Font("Consolas", 16);
            Brush brush = Brushes.White;

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    string tile = "[ ]";  // Placeholder for board state
                    g.DrawString(tile, font, brush, col * TileSize, row * TileSize);
                }
            }
        }
    }
}
