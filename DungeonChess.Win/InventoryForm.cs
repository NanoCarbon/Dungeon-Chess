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
        private const int ShopRows = 8;
        private const int ShopCols = 8;
        private const int InventoryRows = 2;
        private const int InventoryCols = 8;

        // Dictionaries for board data.
        private Dictionary<Point, string> shopItems;      // Placeholder for shop board.
        private Dictionary<Point, string> inventoryItems; // Derived from board state for player1.

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

            // Initialize the shop board with placeholder items.
            InitializeShopPlaceholders();

            // Load player 1's inventory from the board state JSON.
            LoadInventoryFromBoardState(boardStateJson);

            SetupLayout();
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
            shopPanel.Size = new Size(400, 400); // 8 * 50
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
            shopItemLabel.TextAlign = ContentAlignment.MiddleCenter;
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
            inventoryItemLabel.TextAlign = ContentAlignment.MiddleCenter;
            inventoryItemLabel.Text = "No inventory item selected.";
            inventoryItemLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.Controls.Add(inventoryItemLabel);
        }

        private void InitializeShopPlaceholders()
        {
            shopItems = new Dictionary<Point, string>();
            for (int r = 0; r < ShopRows; r++)
            {
                for (int c = 0; c < ShopCols; c++)
                {
                    shopItems[new Point(c, r)] = $"Item {r * ShopCols + c + 1}";
                }
            }
        }

        private void LoadInventoryFromBoardState(string boardStateJson)
        {
            // Deserialize the board state using the same BoardState class.
            BoardState state = JsonSerializer.Deserialize<BoardState>(boardStateJson);
            // Initialize the inventory dictionary with empty strings.
            inventoryItems = new Dictionary<Point, string>();
            for (int r = 0; r < InventoryRows; r++)
            {
                for (int c = 0; c < InventoryCols; c++)
                {
                    inventoryItems[new Point(c, r)] = "";
                }
            }
            // Filter for player1's pieces (assume Player==1).
            // Note: PieceData has properties: Row, Col, Player, Type.
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
                inventoryItems[new Point(col, row)] = symbol;
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
                foreach (var kvp in shopItems)
                {
                    Point cell = kvp.Key;
                    string item = kvp.Value;
                    if (!string.IsNullOrEmpty(item))
                    {
                        SizeF textSize = g.MeasureString(item, font);
                        float x = cell.X * TileSize + (TileSize - textSize.Width) / 2;
                        float y = cell.Y * TileSize + (TileSize - textSize.Height) / 2;
                        g.DrawString(item, font, brush, x, y);
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
                if (shopItems.TryGetValue(selectedShopCell.Value, out string item) && !string.IsNullOrEmpty(item))
                    shopItemLabel.Text = item;
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
                if (inventoryItems.TryGetValue(selectedInventoryCell.Value, out string symbol) && !string.IsNullOrEmpty(symbol))
                    inventoryItemLabel.Text = symbol;
                else
                    inventoryItemLabel.Text = "Empty";
                inventoryPanel.Invalidate();
                inventoryItemLabel.Invalidate();
            }
        }
        #endregion
    }
}
