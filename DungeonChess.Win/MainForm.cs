using System;
using System.Drawing;
using System.Windows.Forms;
using DungeonChess.Core;

namespace DungeonChess.Win
{
    public class MainForm : Form
    {
        private Board board;
        private Label messageLabel;
        private Piece selectedPiece = null;
        private const int TileSize = 50;
        private const int BoardSize = 10;
        public MainForm()
        {
            this.Text = "Dungeon Chess Board";
            this.ClientSize = new Size(TileSize * BoardSize, (TileSize * BoardSize)+100);
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.KeyPreview = true;  // Make sure the form receives key events
            this.KeyDown += MainForm_KeyDown;
            this.MouseClick += MainForm_MouseClick;

            board = new Board(); // Core logic reference
                    
            messageLabel = new Label();
            messageLabel.Text = "Hello World";
            messageLabel.ForeColor = Color.White;
            messageLabel.Font = new Font("Consolas", 12);
            messageLabel.Location = new Point(10, TileSize * BoardSize + 5);  // Below the board
            messageLabel.AutoSize = true;
            Console.WriteLine(board.GetType().FullName);
            this.Controls.Add(messageLabel);
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
                    string tile = "[ ]";
                    var piece = board.GetPieceAt(row, col);
                    
                    if (piece != null)
                    {
                        tile = $"[{piece.Symbol}]";

                        if (piece == selectedPiece)
                            brush = Brushes.Red;  // highlight selected piece
                    }

                    g.DrawString(tile, font, brush, col * TileSize, row * TileSize);
                }
            }

        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                messageLabel.Text = "You pressed P!";
            }
            else if (e.KeyCode == Keys.Escape)
            {
                messageLabel.Text = "Paused!";
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / TileSize;
            int row = e.Y / TileSize;

            if (row >= BoardSize || col >= BoardSize)
            {
                messageLabel.Text = "Click was outside the board.";
                return;
            }

            Piece clickedPiece = board.GetPieceAt(row, col);

            if (selectedPiece == null)
            {
                if (clickedPiece != null)
                {
                    selectedPiece = clickedPiece;
                    messageLabel.Text = $"Selected piece at [{row}, {col}]";
                }
                else
                {
                    messageLabel.Text = "No piece at this tile.";
                }
            }
            else
            {
                // Try to move the piece
                board.MovePiece(selectedPiece, row, col);
                messageLabel.Text = $"Moved piece to [{row}, {col}]";
                selectedPiece = null;

                this.Invalidate();  // force redraw
            }
        }
    }
}
