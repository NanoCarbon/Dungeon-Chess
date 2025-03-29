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
        private Label playerInfoLabel;
        private Piece selectedPiece = null;
        private Button endTurnButton;
        private const int TileSize = 50;
        private const int BoardSize = 10;
        
        public MainForm()
        {
            this.Text = "Dungeon Chess Board";
            this.ClientSize = new Size(TileSize * BoardSize, (TileSize * BoardSize) + 100);
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.MouseClick += MainForm_MouseClick;

            board = new Board(); // Core logic reference
                    
            // Status message label.
            messageLabel = new Label();
            messageLabel.Text = "Hello World";
            messageLabel.ForeColor = Color.White;
            messageLabel.Font = new Font("Consolas", 12);
            messageLabel.Location = new Point(10, TileSize * BoardSize + 5);
            messageLabel.AutoSize = true;
            this.Controls.Add(messageLabel);

            // Player info label.
            playerInfoLabel = new Label();
            playerInfoLabel.ForeColor = Color.White;
            playerInfoLabel.Font = new Font("Consolas", 12);
            playerInfoLabel.AutoSize = true;
            playerInfoLabel.Location = new Point(10, TileSize * BoardSize + 30);
            this.Controls.Add(playerInfoLabel);
            UpdatePlayerInfoLabel();

            // End Turn button.
            endTurnButton = new Button();
            endTurnButton.Text = "End Turn";
            endTurnButton.Font = new Font("Consolas", 12);
            endTurnButton.Size = new Size(100, 40);
            endTurnButton.Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 50);
            endTurnButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            endTurnButton.ForeColor = Color.White;
            endTurnButton.BackColor = Color.Gray;
            endTurnButton.Click += EndTurnButton_Click;
            this.Controls.Add(endTurnButton);
        }

        private void UpdatePlayerInfoLabel()
        {
            playerInfoLabel.Text = $"Turn: {(board.currentPlayer == board.player1 ? "Player 1" : "Player 2")} | Energy: {board.currentPlayer.Energy} | HP: {board.currentPlayer.HP}";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Font font = new Font("Consolas", 16);

            // Draw possible move highlights if a piece is selected.
            if (selectedPiece != null)
            {
                int selRow = selectedPiece.Row;
                int selCol = selectedPiece.Col;
                for (int row = 0; row < BoardSize; row++)
                {
                    for (int col = 0; col < BoardSize; col++)
                    {
                        if (row == selRow && col == selCol)
                            continue;
                        int dx = Math.Abs(row - selRow);
                        int dy = Math.Abs(col - selCol);
                        int distance = Math.Max(dx, dy);
                        if (distance <= selectedPiece.MovementRange)
                        {
                            var occupant = board.GetPieceAt(row, col);
                            Color highlightColor = occupant != null && occupant != selectedPiece
                                ? Color.FromArgb(200, 255, 102, 102)    // Light red for blocked moves.
                                : Color.FromArgb(200, 173, 216, 230);   // Light blue for available moves.
                            using (SolidBrush sb = new SolidBrush(highlightColor))
                            {
                                g.FillRectangle(sb, col * TileSize, row * TileSize, TileSize, TileSize);
                            }
                        }
                    }
                }
            }

            // Draw the board grid and pieces.
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    string tileText = "[ ]";
                    var piece = board.GetPieceAt(row, col);
                    Brush textBrush = Brushes.White;
                    if (piece != null)
                    {
                        tileText = $"[{piece.Symbol}]";
                        if (piece == selectedPiece)
                            textBrush = Brushes.Red;
                        else
                            textBrush = new SolidBrush(piece.GetPlayer().PieceColor);
                    }
                    g.DrawString(tileText, font, textBrush, col * TileSize, row * TileSize);
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
                selectedPiece = null;
                messageLabel.Text = "No piece selected.";
                this.Invalidate();
            }
        }

        private void EndTurnButton_Click(object sender, EventArgs e)
        {
            board.EndTurn();
            selectedPiece = null;
            messageLabel.Text = "Turn ended.";
            UpdatePlayerInfoLabel();
            this.Invalidate();
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

            if (selectedPiece == null)
            {
                Piece clickedPiece = board.GetPieceAt(row, col);
                if (clickedPiece != null)
                {
                    if (clickedPiece.GetPlayer() == board.currentPlayer)
                    {
                        selectedPiece = clickedPiece;
                        messageLabel.Text = $"Selected piece at [{row}, {col}] - HP: {clickedPiece.GetHP()}, Range: {clickedPiece.MovementRange}";
                        this.Invalidate();
                    }
                    else
                    {
                        messageLabel.Text = "Not your piece!";
                    }
                }
                else
                {
                    messageLabel.Text = "No piece at this tile.";
                }
            }
            else
            {
                Piece destinationPiece = board.GetPieceAt(row, col);
                if (destinationPiece != null && destinationPiece != selectedPiece)
                {
                    messageLabel.Text = "Move not allowed because another piece is already there!";
                    return;
                }

                bool moveSuccessful = board.MovePiece(selectedPiece, row, col);
                if (moveSuccessful)
                {
                    messageLabel.Text = $"Moved piece to [{row}, {col}]. Remaining Energy: {board.currentPlayer.Energy}";
                    selectedPiece = null;
                    UpdatePlayerInfoLabel();
                    this.Invalidate();
                }
                else
                {
                    messageLabel.Text = "Move not allowed! Either not your turn, move out of range, no energy, or destination is the same as current position.";
                }
            }
        }
    }
}
