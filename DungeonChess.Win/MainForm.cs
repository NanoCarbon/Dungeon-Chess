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
        private const int BoardSize = 8; // Updated to 8x8 board.
        
        public MainForm()
        {
            // Set form title and client size: width is doubled.
            this.Text = "Dungeon Chess Board";
            this.ClientSize = new Size(TileSize * BoardSize * 2, (TileSize * BoardSize) + 100);
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.MouseClick += MainForm_MouseClick;

            board = new Board(); // Core logic reference.
                    
            // Status message label.
            messageLabel = new Label();
            messageLabel.Text = "Welcome to Dungeon Chess!";
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

            // 1. Draw the base board with alternating colors.
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    // Alternate tile color: (row+col) even = black, odd = white.
                    Color tileColor = ((row + col) % 2 == 0) ? Color.Black : Color.White;
                    using (SolidBrush sb = new SolidBrush(tileColor))
                    {
                        g.FillRectangle(sb, col * TileSize, row * TileSize, TileSize, TileSize);
                    }
                }
            }

            // 2. Draw possible move highlights if a piece is selected.
            if (selectedPiece != null)
            {
                int selRow = selectedPiece.Row;
                int selCol = selectedPiece.Col;
                for (int row = 0; row < BoardSize; row++)
                {
                    for (int col = 0; col < BoardSize; col++)
                    {
                        // Skip the tile where the piece is.
                        if (row == selRow && col == selCol)
                            continue;

                        bool isValidMove = false;
                        if (selectedPiece.MovementBehavior != null)
                        {
                            isValidMove = selectedPiece.MovementBehavior.IsMoveValid(selectedPiece, row, col, board);
                        }
                        else
                        {
                            int dx = Math.Abs(row - selRow);
                            int dy = Math.Abs(col - selCol);
                            int distance = Math.Max(dx, dy);
                            isValidMove = (distance <= selectedPiece.MovementRange);
                        }

                        if (isValidMove)
                        {
                            var occupant = board.GetPieceAt(row, col);
                            Color highlightColor = (occupant != null && occupant != selectedPiece)
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

            // 3. Draw the board pieces.
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    string tileText = " ";
                    var piece = board.GetPieceAt(row, col);
                    Brush textBrush = Brushes.White;
                    if (piece != null)
                    {
                        tileText = $"{piece.Symbol}";
                        if (piece == selectedPiece)
                            textBrush = Brushes.Red;
                        else
                            textBrush = new SolidBrush(piece.GetPlayer().PieceColor);
                    }
                    
                    // Measure the size of the text.
                    SizeF textSize = g.MeasureString(tileText, font);
                    
                    // Calculate the center position of the tile.
                    float x = col * TileSize + (TileSize - textSize.Width) / 2;
                    float y = row * TileSize + (TileSize - textSize.Height) / 2;
                    
                    g.DrawString(tileText, font, textBrush, x, y);
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
            
            if (e.Button == MouseButtons.Right)
            {
                // Right-click: Attempt an attack.
                if (selectedPiece == null)
                {
                    messageLabel.Text = "No piece selected for attack.";
                    return;
                }

                // Check that the current player has enough energy.
                if (board.currentPlayer.Energy <= 0)
                {
                    messageLabel.Text = "Not enough energy to attack.";
                    return;
                }

                Piece targetPiece = board.GetPieceAt(row, col);
                if (targetPiece == null)
                {
                    messageLabel.Text = "No target piece at this tile.";
                    return;
                }
                
                int dx = Math.Abs(row - selectedPiece.Row);
                int dy = Math.Abs(col - selectedPiece.Col);
                int distance = Math.Max(dx, dy);
                if (distance > selectedPiece.MovementRange)
                {
                    messageLabel.Text = "Target is out of attack range.";
                    return;
                }
                
                if (targetPiece.GetPlayer() == selectedPiece.GetPlayer())
                {
                    messageLabel.Text = "Cannot attack your own piece.";
                    return;
                }
                
                targetPiece.TakeDamage(selectedPiece.Attack);
                board.currentPlayer.Energy -= 1;
                
                if (targetPiece.GetHP() == 0)
                {
                    messageLabel.Text = $"Attacked piece at [{row}, {col}] for {selectedPiece.Attack} damage. Target piece has died!";
                    // Remove the target piece from the board.
                    board.Pieces.Remove(targetPiece);
                    // If the attacker is NOT ranged, move it into the target's square.
                    if (!selectedPiece.IsRanged)
                    {
                        selectedPiece.Row = row;
                        selectedPiece.Col = col;
                    }
                }
                else
                {
                    messageLabel.Text = $"Attacked piece at [{row}, {col}] for {selectedPiece.Attack} damage. Remaining Energy: {board.currentPlayer.Energy}";
                }
                UpdatePlayerInfoLabel();
                this.Invalidate();
                return;
            }
            else if (e.Button == MouseButtons.Left)
            {
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
}
