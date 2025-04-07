using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using DungeonChess.Core;  // For BoardState and PieceData

namespace DungeonChess.Win
{
    public class InventoryForm : Form
    {
        // Panels for the two distinct boards.
        private Panel shopPanel;         // Shop board: 8x8 grid.
        private Panel inventoryPanel;    // Inventory board: 2x8 grid.
        private Dictionary<Point, PieceData> inventoryPieceData;

        // Top labels for each board.
        private Label shopTitleLabel;
        private Label inventoryTitleLabel;

        // Bottom labels for selected cell details.
        private Label shopItemLabel;
        private Label inventoryItemLabel;

        // Layout constants.
        private const int ClientWidth = 850;
        private const int ClientHeight = 850;
        private const int ColumnWidth = 425;  // Each column is 425px wide.
        private const int TopLabelHeight = 50;
        private const int BottomLabelHeight = 100;
        private const int TileSize = 50;

        // Grid dimensions.
        private const int ShopRows = 2;
        private const int ShopCols = 8;
        private const int InventoryRows = 2;
        private const int InventoryCols = 8;

        // Dictionaries for board data.
        private Dictionary<Point, Piece> shopPieces;
        // The inventoryItems dictionary remains (populated from board state).
        private Dictionary<Point, string> inventoryItems;

        // To hold player's gold (extracted from BoardState).
        private int? currentGoldValue = null;

        // Selected cells.
        private Point? selectedShopCell = null;
        private Point? selectedInventoryCell = null;

        // The board state JSON passed from MainForm.
        private string boardStateJson;

        public InventoryForm(string boardStateJson)
        {
            this.boardStateJson = boardStateJson;
            this.Text = "Inventory";
            this.ClientSize = new Size(ClientWidth, ClientHeight);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;

            // Button returnToMainButton = new Button();
            // returnToMainButton.Text = "Return to Main";
            // returnToMainButton.Font = new Font("Arial", 12, FontStyle.Bold);
            // returnToMainButton.Size = new Size(120, 40);
            // returnToMainButton.Location = new Point(this.ClientSize.Width - 130, this.ClientSize.Height - 60);
            // returnToMainButton.ForeColor = Color.Black;
            // returnToMainButton.BackColor = Color.LightGray;
            // returnToMainButton.Click += (sender, e) =>
            // {
            //     // If InventoryForm was shown with ShowDialog, setting DialogResult will close it.
            //     this.DialogResult = DialogResult.OK;
            //     this.Close();
            // };
            // this.Controls.Add(returnToMainButton);


            // Initialize the shop board with placeholder items.
            InitializeShopPlaceholders();

            // Load player 1's inventory from the board state JSON.
            LoadInventoryFromBoardState(boardStateJson);

            SetupLayout();

            // // Add a label for player's gold below the inventory panel (or details).
            // Label goldLabel = new Label();
            // goldLabel.Size = new Size(ColumnWidth, 50); // 50px tall.
            // goldLabel.Location = new Point(ColumnWidth, inventoryItemLabel.Top - 50); // Place it just above the inventory detail label.
            // goldLabel.BackColor = Color.DarkGray;
            // goldLabel.ForeColor = Color.Black;
            // goldLabel.Font = new Font("Arial", 10, FontStyle.Regular);
            // goldLabel.TextAlign = ContentAlignment.MiddleCenter;
            // // Update the gold label text using the deserialized board state.
            // // Assuming that LoadInventoryFromBoardState sets currentGoldValue (extracted from state.Player1.Gold).
            // goldLabel.Text = "Gold: " + (currentGoldValue.HasValue ? currentGoldValue.Value.ToString() : "N/A");
            // this.Controls.Add(goldLabel);

            // "Return to Start" button for the shop side.
            Button returnToStartButton = new Button();
            returnToStartButton.Text = "Return to Start";
            returnToStartButton.Font = new Font("Arial", 10, FontStyle.Bold);
            returnToStartButton.Size = new Size(120, 40);
            returnToStartButton.ForeColor = Color.Black;
            returnToStartButton.BackColor = Color.LightGray;
            // Position it in the left column above the shopItemLabel (with a 10px gap).
            returnToStartButton.Location = new Point((ColumnWidth - returnToStartButton.Width) / 2, shopItemLabel.Top - returnToStartButton.Height - 10);
            returnToStartButton.Click += (sender, e) =>
            {
                // When clicked, return to the Start form.
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(returnToStartButton);

            // Add a "Continue" button for the inventory side.
            // (For now, it does nothing.)
            Button continueButton = new Button();
            continueButton.Text = "Continue";
            continueButton.Font = new Font("Arial", 12, FontStyle.Bold);
            continueButton.Size = new Size(120, 40);
            continueButton.ForeColor = Color.Black;
            continueButton.BackColor = Color.LightGray;
            // Position it below the inventory detail label.
            continueButton.Location = new Point(ColumnWidth + (ColumnWidth - continueButton.Width) / 2, inventoryItemLabel.Top - continueButton.Height - 10);
            continueButton.Click += (sender, e) =>
            {
                // For now, do nothing.
            };
            this.Controls.Add(continueButton);

        }

        private void SetupLayout()
        {
            // --- Left Column: Shop ---
            // Top label.
            shopTitleLabel = new Label();
            shopTitleLabel.Size = new Size(ColumnWidth, TopLabelHeight);
            shopTitleLabel.Location = new Point(0, 0);
            shopTitleLabel.Text = "Shop";
            shopTitleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            shopTitleLabel.ForeColor = Color.Black;
            shopTitleLabel.BackColor = Color.LightGray;
            shopTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            shopTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(shopTitleLabel);

            // Shop board panel: 8x8 grid.
            shopPanel = new Panel();
            shopPanel.Size = new Size(400, 100); // 8 * 50
            // Center horizontally in the left column.
            int shopPanelX = (ColumnWidth - shopPanel.Width) / 2;
            // Place immediately below the top label.
            int shopPanelY = TopLabelHeight;
            shopPanel.Location = new Point(shopPanelX, shopPanelY);
            shopPanel.BackColor = Color.White;
            shopPanel.Paint += ShopPanel_Paint;
            shopPanel.MouseClick += ShopPanel_MouseClick;
            shopPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.Controls.Add(shopPanel);

            // Bottom label for shop details.
            shopItemLabel = new Label();
            shopItemLabel.Size = new Size(ColumnWidth, BottomLabelHeight);
            shopItemLabel.Location = new Point(0, ClientHeight - BottomLabelHeight);
            shopItemLabel.BackColor = Color.DarkGray;
            shopItemLabel.ForeColor = Color.Black;
            shopItemLabel.Font = new Font("Arial", 10, FontStyle.Regular);
            shopItemLabel.TextAlign = ContentAlignment.MiddleLeft;  // Left-aligned
            shopItemLabel.Text = "No shop item selected.";
            shopItemLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(shopItemLabel);


            // --- Right Column: Inventory ---
            // Top label.
            inventoryTitleLabel = new Label();
            inventoryTitleLabel.Size = new Size(ColumnWidth, TopLabelHeight);
            inventoryTitleLabel.Location = new Point(ColumnWidth, 0);
            inventoryTitleLabel.Text = "Inventory";
            inventoryTitleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            inventoryTitleLabel.ForeColor = Color.Black;
            inventoryTitleLabel.BackColor = Color.LightGray;
            inventoryTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            inventoryTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(inventoryTitleLabel);

            // Inventory board panel: 2x8 grid.
            inventoryPanel = new Panel();
            inventoryPanel.Size = new Size(400, 100); // 8 columns * 50, 2 rows * 50
            // Center horizontally in the right column.
            int invPanelX = ColumnWidth + ((ColumnWidth - inventoryPanel.Width) / 2);
            int invPanelY = TopLabelHeight;
            inventoryPanel.Location = new Point(invPanelX, invPanelY);
            inventoryPanel.BackColor = Color.White;
            inventoryPanel.Paint += InventoryPanel_Paint;
            inventoryPanel.MouseClick += InventoryPanel_MouseClick;
            inventoryPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(inventoryPanel);

            // Bottom label for inventory details.
            inventoryItemLabel = new Label();
            inventoryItemLabel.Size = new Size(ColumnWidth, BottomLabelHeight);
            inventoryItemLabel.Location = new Point(ColumnWidth, ClientHeight - BottomLabelHeight);
            inventoryItemLabel.BackColor = Color.DarkGray;
            inventoryItemLabel.ForeColor = Color.Black;
            inventoryItemLabel.Font = new Font("Arial", 10, FontStyle.Regular);
            inventoryItemLabel.TextAlign = ContentAlignment.MiddleLeft;  // Left-aligned
            inventoryItemLabel.Text = "No inventory item selected.";
            inventoryItemLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(inventoryItemLabel);
        }

        private void InitializeShopPlaceholders()
        {
            shopPieces = new Dictionary<Point, Piece>();
            // For a 2x8 grid, initialize each cell to null.
            for (int r = 0; r < ShopRows; r++)
            {
                for (int c = 0; c < ShopCols; c++)
                {
                    shopPieces[new Point(c, r)] = null;
                }
            }
            // Create a dummy shop player.
            Player shopPlayer = new Player();
            shopPlayer.PieceColor = Color.Gray;  // Shop pieces color.

            // Populate specific cells with actual pieces.
            shopPieces[new Point(0, 0)] = new Piece(0, 0, shopPlayer, PieceType.Pawn);
            shopPieces[new Point(1, 0)] = new Piece(0, 0, shopPlayer, PieceType.Bishop);
            shopPieces[new Point(2, 0)] = new Piece(0, 0, shopPlayer, PieceType.Rook);
        }

        private void LoadInventoryFromBoardState(string boardStateJson)
        {
            // Deserialize the board state using the same BoardState class.
            BoardState state = JsonSerializer.Deserialize<BoardState>(boardStateJson);
            // Assume state.Player1 now includes a Gold property.
            currentGoldValue = state.Player1.Gold;

            // Initialize the inventory dictionaries with empty strings.
            inventoryItems = new Dictionary<Point, string>();
            inventoryPieceData = new Dictionary<Point, PieceData>();
            for (int r = 0; r < InventoryRows; r++)
            {
                for (int c = 0; c < InventoryCols; c++)
                {
                    Point pt = new Point(c, r);
                    inventoryItems[pt] = "";
                    // Initialize with null to start.
                    inventoryPieceData[pt] = null;
                }
            }
            // Filter for player1's pieces (assume Player==1).
            var player1Pieces = state.Pieces.FindAll(p => p.Player == 1);
            int index = 0;
            foreach (var piece in player1Pieces)
            {
                if (index >= InventoryRows * InventoryCols)
                    break;
                int row = index / InventoryCols;
                int col = index % InventoryCols;
                // Use a helper to convert piece type to its symbol.
                string symbol = GetSymbolFromType(piece.Type);
                Point key = new Point(col, row);
                inventoryItems[key] = symbol;
                // Also store the full PieceData for later details.
                inventoryPieceData[key] = piece;
                index++;
            }
        }


        private string GetSymbolFromType(string type)
        {
            switch (type)
            {
                case "Pawn": return "P";
                case "King": return "K";
                case "Queen": return "Q";
                case "Bishop": return "B";
                case "Knight": return "N";
                case "Rook": return "R";
                default: return type.Substring(0, 1);
            }
        }

        #region Shop Panel Drawing and Interaction
        private void ShopPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen pen = new Pen(Color.Black))
            {
                for (int row = 0; row <= ShopRows; row++)
                    g.DrawLine(pen, 0, row * TileSize, ShopCols * TileSize, row * TileSize);
                for (int col = 0; col <= ShopCols; col++)
                    g.DrawLine(pen, col * TileSize, 0, col * TileSize, ShopRows * TileSize);
            }
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                foreach (var kvp in shopPieces)
                {
                    Point cell = kvp.Key;
                    Piece piece = kvp.Value;
                    if (piece != null)
                    {
                        string symbol = piece.Symbol.ToString();
                        SizeF textSize = g.MeasureString(symbol, font);
                        float x = cell.X * TileSize + (TileSize - textSize.Width) / 2;
                        float y = cell.Y * TileSize + (TileSize - textSize.Height) / 2;
                        g.DrawString(symbol, font, brush, x, y);
                    }
                }
            }
            if (selectedShopCell.HasValue)
            {
                int col = selectedShopCell.Value.X;
                int row = selectedShopCell.Value.Y;
                Rectangle rect = new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);
                using (Pen highlightPen = new Pen(Color.Yellow, 2))
                {
                    g.DrawRectangle(highlightPen, rect);
                }
            }
        }


        private void ShopPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / TileSize;
            int row = e.Y / TileSize;
            if (col < ShopCols && row < ShopRows)
            {
                selectedShopCell = new Point(col, row);
                if (shopPieces.TryGetValue(selectedShopCell.Value, out Piece piece) && piece != null)
                {
                    // Update with detailed attributes and player's gold.
                    // currentGoldValue should have been set during LoadInventoryFromBoardState.
                    shopItemLabel.Text = $"Type: {piece.Type}\n" +
                                        $"HP: {piece.hp}\n" +
                                        $"Attack: {piece.Attack}\n" +
                                        $"Movement: {piece.MovementRange}\n" +
                                        $"Piece Value: {piece.PieceValue}\n" +
                                        $"Player Gold: {(currentGoldValue.HasValue ? currentGoldValue.Value.ToString() : "N/A")}";
                }
                else
                    shopItemLabel.Text = "Empty";
                shopPanel.Invalidate();
            }
        }


        #endregion

        #region Inventory Panel Drawing and Interaction
        private void InventoryPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen pen = new Pen(Color.Black))
            {
                for (int row = 0; row <= InventoryRows; row++)
                    g.DrawLine(pen, 0, row * TileSize, InventoryCols * TileSize, row * TileSize);
                for (int col = 0; col <= InventoryCols; col++)
                    g.DrawLine(pen, col * TileSize, 0, col * TileSize, InventoryRows * TileSize);
            }
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                foreach (var kvp in inventoryItems)
                {
                    Point cell = kvp.Key;
                    string symbol = kvp.Value;
                    if (!string.IsNullOrEmpty(symbol))
                    {
                        SizeF textSize = g.MeasureString(symbol, font);
                        float x = cell.X * TileSize + (TileSize - textSize.Width) / 2;
                        float y = cell.Y * TileSize + (TileSize - textSize.Height) / 2;
                        g.DrawString(symbol, font, brush, x, y);
                    }
                }
            }
            if (selectedInventoryCell.HasValue)
            {
                int col = selectedInventoryCell.Value.X;
                int row = selectedInventoryCell.Value.Y;
                Rectangle rect = new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);
                using (Pen highlightPen = new Pen(Color.Yellow, 2))
                {
                    g.DrawRectangle(highlightPen, rect);
                }
            }
        }

        private void InventoryPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / TileSize;
            int row = e.Y / TileSize;
            if (col < InventoryCols && row < InventoryRows)
            {
                selectedInventoryCell = new Point(col, row);
                // If we have PieceData for the selected cell, display detailed info.
                if (inventoryPieceData != null &&
                    inventoryPieceData.TryGetValue(selectedInventoryCell.Value, out PieceData pd) &&
                    pd != null)
                {
                    inventoryItemLabel.Text = $"Type: {pd.Type}\n" +
                                                $"HP: {pd.HP}\n" +
                                                $"Movement Range: {pd.MovementRange}\n" +
                                                $"Attack: {pd.Attack}\n" +
                                                $"Attack Range: {pd.AttackRange}\n" +
                                                $"Value: {pd.PieceValue}";
                }
                else
                {
                    inventoryItemLabel.Text = "Empty";
                }
                inventoryPanel.Invalidate();
                inventoryItemLabel.Invalidate();
            }
        }



        #endregion
    }
}
