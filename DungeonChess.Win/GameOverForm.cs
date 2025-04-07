using System;
using System.Drawing;
using System.Windows.Forms;

namespace DungeonChess.Win
{
    public class GameOverForm : Form
    {
        private string boardStateJson; // Store the serialized board state.
        
        public GameOverForm(int winningPlayer, string boardStateJson)
        {
            this.boardStateJson = boardStateJson;
            this.Text = "Game Over";
            this.Size = new Size(300, 250); // Increased height for two buttons.
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Winner message label.
            Label message = new Label();
            message.Text = $"Player {winningPlayer} won!";
            message.Font = new Font("Arial", 16, FontStyle.Bold);
            message.ForeColor = Color.White;
            message.AutoSize = false;
            message.TextAlign = ContentAlignment.MiddleCenter;
            message.Dock = DockStyle.Top;
            message.Height = 80;
            this.Controls.Add(message);

            // "Go to Inventory" button.
            Button inventoryButton = new Button();
            inventoryButton.Text = "Go to Inventory";
            inventoryButton.Font = new Font("Arial", 12, FontStyle.Bold);
            inventoryButton.ForeColor = Color.White;
            inventoryButton.BackColor = Color.Gray;
            inventoryButton.Size = new Size(150, 40);
            // Place it below the message label.
            inventoryButton.Location = new Point((this.ClientSize.Width - inventoryButton.Width) / 2, message.Bottom + 10);
            inventoryButton.Anchor = AnchorStyles.None;
            inventoryButton.Click += (sender, e) =>
            {
                InventoryForm invForm = new InventoryForm(this.boardStateJson);
                invForm.ShowDialog(this);
            };
            this.Controls.Add(inventoryButton);

            // "Return to Start" button placed below the inventory button.
            Button returnButton = new Button();
            returnButton.Text = "Return to Start";
            returnButton.Font = new Font("Arial", 10, FontStyle.Bold);
            returnButton.ForeColor = Color.White;
            returnButton.BackColor = Color.Gray;
            returnButton.Size = new Size(150, 40);
            returnButton.Location = new Point((this.ClientSize.Width - returnButton.Width) / 2, inventoryButton.Bottom + 10);
            returnButton.Anchor = AnchorStyles.None;
            returnButton.Click += (sender, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(returnButton);
        }
    }
}
