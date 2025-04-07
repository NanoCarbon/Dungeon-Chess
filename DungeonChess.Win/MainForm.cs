using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;
using DungeonChess.Core;

namespace DungeonChess.Win
{
    public partial class MainForm : Form
    {
        // Event to notify when the game is over.
        public event EventHandler<int> GameOver;
        private Board board;
        private string saveFileName;
        private Label messageLabel;
        private Label playerInfoLabel;
        private Piece selectedPiece = null;
        private Button endTurnButton;
        private CheckBox autoEndTurnCheckBox; // New checkbox for auto end turn
        private const int TileSize = 50;
        private const int BoardSize = 8; // 8x8 board
        // New fields to record if each player started with a king.
        private bool player1StartedWithKing;
        private bool player2StartedWithKing;

        public MainForm(string saveFileName)
        {
            // Set form title and client size: width is doubled.
            this.saveFileName = saveFileName;
            this.Text = "Dungeon Chess Board";
            this.ClientSize = new Size(850, 850); // updated size
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.MouseClick += MainForm_MouseClick;

            board = new Board(saveFileName); // Core logic reference

            // Record the starting king status.
            player1StartedWithKing = board.Pieces.Any(p => p.player == board.player1 && p.Type == PieceType.King);
            player2StartedWithKing = board.Pieces.Any(p => p.player == board.player2 && p.Type == PieceType.King);

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

             // Auto End Turn CheckBox
            autoEndTurnCheckBox = new CheckBox();
            autoEndTurnCheckBox.Text = "Auto End Turn";
            autoEndTurnCheckBox.Font = new Font("Consolas", 12);
            autoEndTurnCheckBox.ForeColor = Color.White;
            autoEndTurnCheckBox.BackColor = Color.Black;
            autoEndTurnCheckBox.AutoSize = true;
            // Position it above the End Turn button.
            autoEndTurnCheckBox.Location = new Point(this.ClientSize.Width - 150, endTurnButton.Top - 30);
            this.Controls.Add(autoEndTurnCheckBox);
            
            // Create the "Return to Start" button.
            Button returnToStartButton = new Button();
            returnToStartButton.Text = "Return to Start";
            returnToStartButton.Font = new Font("Consolas", 12);
            returnToStartButton.Size = new Size(120, 40);
            // Position it near the left-bottom corner.
            returnToStartButton.Location = new Point(10, this.ClientSize.Height - 60);
            returnToStartButton.ForeColor = Color.White;
            returnToStartButton.BackColor = Color.Gray;
            returnToStartButton.Click += (sender, e) =>
            {
                // This will close the main form.
                this.Close();
            };
            this.Controls.Add(returnToStartButton);

            // Create the "Save Game" button.
            Button saveGameButton = new Button();
            saveGameButton.Text = "Save Game";
            saveGameButton.Font = new Font("Consolas", 12);
            saveGameButton.Size = new Size(120, 40);
            // Place it to the right of the Return button with a gap of 10 pixels.
            saveGameButton.Location = new Point(returnToStartButton.Right + 10, this.ClientSize.Height - 60);
            saveGameButton.ForeColor = Color.White;
            saveGameButton.BackColor = Color.Gray;
            saveGameButton.Click += SaveGameButton_Click; // Assuming this method is defined.
            this.Controls.Add(saveGameButton);

        //     // Create the "Inventory" button.
        //     Button inventoryButton = new Button();
        //     inventoryButton.Text = "Inventory";
        //     inventoryButton.Font = new Font("Consolas", 12);
        //     inventoryButton.Size = new Size(120, 40);
        //     // Place it to the right of the Save Game button with a gap of 10 pixels.
        //     inventoryButton.Location = new Point(saveGameButton.Right + 10, this.ClientSize.Height - 60);
        //     inventoryButton.ForeColor = Color.White;
        //     inventoryButton.BackColor = Color.Gray;
        //     inventoryButton.Click += (sender, e) =>
        //     {
        //         // Example: In MainForm, when opening inventory:
        //         string boardStateJson = JsonSerializer.Serialize(board.GetBoardState(), new JsonSerializerOptions { WriteIndented = true });
        //         InventoryForm invForm = new InventoryForm(boardStateJson);
        //         invForm.ShowDialog(this);
        //     };
        //     this.Controls.Add(inventoryButton);
        }

        public string GetBoardStateJson()
        {
            BoardState state = board.GetBoardState();
            return JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        }

        private void UpdatePlayerInfoLabel()
        {
            // Highlight the label with the current player's color.
            playerInfoLabel.ForeColor = board.currentPlayer.PieceColor;
            playerInfoLabel.Text = $"Turn: {(board.currentPlayer == board.player1 ? "Player 1" : "Player 2")}\nEnergy: {board.currentPlayer.Energy} | HP: {board.currentPlayer.HP}";
        }

        // Helper to check auto end turn condition.
        private void CheckAutoEndTurn()
        {
            if (autoEndTurnCheckBox != null && autoEndTurnCheckBox.Checked && board.currentPlayer.Energy <= 0)
            {
                board.EndTurn();
                messageLabel.Text = "Turn auto-ended.";
                UpdatePlayerInfoLabel();
                this.Invalidate();
            }
        }

        // In your win condition check, instead of directly closing and creating a new StartForm,
        // raise the GameOver event:
        private void CheckWinCondition()
        {
            bool player1HasKing = board.Pieces.Any(p => p.player == board.player1 && p.Type == PieceType.King);
            bool player2HasKing = board.Pieces.Any(p => p.player == board.player2 && p.Type == PieceType.King);
            bool player1HasAnyPieces = board.Pieces.Any(p => p.player == board.player1);
            bool player2HasAnyPieces = board.Pieces.Any(p => p.player == board.player2);

            if (!player1HasAnyPieces || (player1StartedWithKing && !player1HasKing))
            {
                // Raise event for game over with Player 2 winning.
                GameOver?.Invoke(this, 2);
            }
            else if (!player2HasAnyPieces || (player2StartedWithKing && !player2HasKing))
            {
                // Raise event for game over with Player 1 winning.
                GameOver?.Invoke(this, 1);
            }
        }

        // Helper method to show the game over popup.
        private void ShowGameOver(int winningPlayer)
        {
            // Serialize the current board state.
            string boardStateJson = JsonSerializer.Serialize(board.GetBoardState(), new JsonSerializerOptions { WriteIndented = true });
            // Pass the board state JSON to GameOverForm.
            GameOverForm gameOverForm = new GameOverForm(winningPlayer, boardStateJson);
            gameOverForm.ShowDialog();
            this.Close();
            StartForm startForm = new StartForm();
            startForm.Show();
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
                        textBrush = piece == selectedPiece ? Brushes.Red : new SolidBrush(piece.player.PieceColor);
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

                if (targetPiece.player == selectedPiece.player)
                {
                    messageLabel.Text = "Cannot attack your own piece.";
                    return;
                }
                
                // Perform the attack.
                targetPiece.TakeDamage(selectedPiece.Attack);
                board.currentPlayer.Energy -= 1;
                
                if (targetPiece.hp == 0)
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
                CheckAutoEndTurn();
                CheckWinCondition();
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
                        if (clickedPiece.player == board.currentPlayer)
                        {
                            selectedPiece = clickedPiece;
                            messageLabel.Text = $"Selected piece at [{row}, {col}] - HP: {clickedPiece.hp}, Range: {clickedPiece.MovementRange}";
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
                        CheckAutoEndTurn();
                        CheckWinCondition();
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
