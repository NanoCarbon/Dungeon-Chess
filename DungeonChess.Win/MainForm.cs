using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
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
        private const int BoardSize = 8; // 8x8 board

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

            board = new Board(); // Core logic reference

            // -------------------------
            // 1. MESSAGE LABEL (TOP-RIGHT)
            // -------------------------
            messageLabel = new Label();
            messageLabel.Text = "Welcome to Dungeon Chess!";
            messageLabel.ForeColor = Color.White;
            messageLabel.Font = new Font("Consolas", 12);
            messageLabel.BackColor = Color.Transparent;
            messageLabel.Location = new Point(TileSize * BoardSize + 20, 5);
            messageLabel.Size = new Size((TileSize * BoardSize) - 40, 120);
            messageLabel.AutoSize = false;
            messageLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(messageLabel);

            // -------------------------
            // 2. PLAYER INFO LABEL (below messageLabel)
            // -------------------------
            playerInfoLabel = new Label();
            playerInfoLabel.ForeColor = Color.White;
            playerInfoLabel.Font = new Font("Consolas", 12);
            playerInfoLabel.BackColor = Color.Transparent;
            playerInfoLabel.Location = new Point(TileSize * BoardSize + 20, messageLabel.Bottom + 5);
            playerInfoLabel.Size = new Size((TileSize * BoardSize) - 40, 40);
            playerInfoLabel.AutoSize = false;
            playerInfoLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(playerInfoLabel);
            UpdatePlayerInfoLabel();

            // -------------------------
            // 3. END TURN BUTTON (BOTTOM-RIGHT)
            // -------------------------
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

            // -------------------------
            // 4. SAVE GAME BUTTON (BOTTOM-LEFT)
            // -------------------------
            Button saveGameButton = new Button();
            saveGameButton.Text = "Save Game";
            saveGameButton.Font = new Font("Consolas", 12);
            saveGameButton.Size = new Size(100, 40);
            saveGameButton.Location = new Point(10, this.ClientSize.Height - 50);
            saveGameButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            saveGameButton.ForeColor = Color.White;
            saveGameButton.BackColor = Color.Gray;
            saveGameButton.Click += SaveGameButton_Click;
            this.Controls.Add(saveGameButton);
        }

        private void UpdatePlayerInfoLabel()
        {
            playerInfoLabel.Text = $"Turn: {(board.currentPlayer == board.player1 ? "Player 1" : "Player 2")}\nEnergy: {board.currentPlayer.Energy} | HP: {board.currentPlayer.HP}";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Font font = new Font("Consolas", 16);

            // 1. Draw the base board (only on the left half) using the tile grid.
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    Color tileColor = board.Tiles[row, col].IsTraversable ? Color.White : board.Tiles[row, col].BackgroundColor;
                    using (SolidBrush sb = new SolidBrush(tileColor))
                    {
                        g.FillRectangle(sb, col * TileSize, row * TileSize, TileSize, TileSize);
                    }
                    using (Pen pen = new Pen(Color.Black))
                    {
                        g.DrawRectangle(pen, col * TileSize, row * TileSize, TileSize, TileSize);
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
                        if (row == selRow && col == selCol)
                            continue;
                        if (!board.Tiles[row, col].IsTraversable)
                            continue;

                        int dx = Math.Abs(row - selRow);
                        int dy = Math.Abs(col - selCol);
                        int distance = Math.Max(dx, dy);

                        bool movementValid = false;
                        bool attackValid = false;

                        if (selectedPiece.MovementBehavior != null)
                            movementValid = selectedPiece.MovementBehavior.IsMoveValid(selectedPiece, row, col, board);
                        else
                            movementValid = (distance <= selectedPiece.MovementRange);

                        if (selectedPiece.AttackBehavior != null)
                            attackValid = selectedPiece.AttackBehavior.IsAttackValid(selectedPiece, row, col, board);
                        else
                            attackValid = (distance <= selectedPiece.AttackRange);

                        Color? highlightColor = null;
                        if (movementValid && attackValid)
                        {
                            highlightColor = Color.FromArgb(200, 255, 255, 224);
                        }
                        else if (!movementValid && attackValid)
                        {
                            highlightColor = Color.FromArgb(200, 144, 238, 144);
                        }
                        else if (movementValid && !attackValid)
                        {
                            highlightColor = Color.FromArgb(200, 173, 216, 230);
                        }

                        if (highlightColor.HasValue)
                        {
                            using (SolidBrush sb = new SolidBrush(highlightColor.Value))
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
                        textBrush = piece == selectedPiece ? Brushes.Red : new SolidBrush(piece.GetPlayer().PieceColor);
                    }
                    
                    SizeF textSize = g.MeasureString(tileText, font);
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
                // Deselect the active piece.
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

        // New event handler to save the game state.
        private void SaveGameButton_Click(object sender, EventArgs e)
        {
            try
            {
                BoardState state = board.GetBoardState();
                string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string saveFolder = Path.Combine(baseDir, "saves");
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
                string saveFilePath = Path.Combine(saveFolder, "save_current.json");
                File.WriteAllText(saveFilePath, json);
                messageLabel.Text = "Game state saved successfully!";
            }
            catch (Exception ex)
            {
                messageLabel.Text = "Error saving game state: " + ex.Message;
            }
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / TileSize;
            int row = e.Y / TileSize;
            
            // If click is outside board area (the board is drawn on the left half), deselect any piece.
            if (row >= BoardSize || col >= BoardSize)
            {
                selectedPiece = null;
                messageLabel.Text = "Click was outside the board. No piece selected.";
                this.Invalidate();
                return;
            }
            
            // Right-click: Attempt an attack.
            if (e.Button == MouseButtons.Right)
            {
                if (selectedPiece == null)
                {
                    messageLabel.Text = "No piece selected for attack.";
                    return;
                }
                
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
                
                // Use the attack behavior if defined; otherwise, fall back to simple distance check.
                bool isAttackValid = false;
                if (selectedPiece.AttackBehavior != null)
                {
                    isAttackValid = selectedPiece.AttackBehavior.IsAttackValid(selectedPiece, row, col, board);
                }
                else
                {
                    int dx = Math.Abs(row - selectedPiece.Row);
                    int dy = Math.Abs(col - selectedPiece.Col);
                    int distance = Math.Max(dx, dy);
                    isAttackValid = (distance <= selectedPiece.AttackRange);
                }

                if (!isAttackValid)
                {
                    messageLabel.Text = "Target is out of attack range.";
                    return;
                }

                if (targetPiece.GetPlayer() == selectedPiece.GetPlayer())
                {
                    messageLabel.Text = "Cannot attack your own piece.";
                    return;
                }
                
                // Perform the attack.
                targetPiece.TakeDamage(selectedPiece.Attack);
                board.currentPlayer.Energy -= 1;
                
                if (targetPiece.GetHP() == 0)
                {
                    messageLabel.Text = $"Attacked piece at [{row}, {col}] for {selectedPiece.Attack} damage. Target piece has died!";
                    board.Pieces.Remove(targetPiece);
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
                if (selectedPiece != null && row == selectedPiece.Row && col == selectedPiece.Col)
                {
                    selectedPiece = null;
                    messageLabel.Text = "No piece selected.";
                    this.Invalidate();
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
                        selectedPiece = null;
                        messageLabel.Text = "No piece at this tile. No piece selected.";
                        this.Invalidate();
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
