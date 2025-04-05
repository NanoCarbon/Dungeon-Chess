using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DungeonChess.Win
{
    public partial class StartForm : Form
    {
        private Button newGameButton;
        private Button loadGameButton;
        private Label titleLabel;
        private TextBox saveFileTextBox;
        // Event to notify when the user requests to start a game.
        public event EventHandler<string> StartGameRequested;

        public StartForm()
        {
            this.Text = "Dungeon Chess - Start";
            this.ClientSize = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            
            // Title Label - spans the entire width for centered text.
            titleLabel = new Label();
            titleLabel.Text = "Dungeon Chess";
            titleLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            titleLabel.Size = new Size(this.ClientSize.Width, 50);
            titleLabel.Location = new Point(0, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.ForeColor = Color.White;
            titleLabel.BackColor = Color.Black;
            this.Controls.Add(titleLabel);
            
            // New Game Button
            newGameButton = new Button();
            newGameButton.Text = "New Game";
            newGameButton.Font = new Font("Arial", 12, FontStyle.Bold);
            newGameButton.Size = new Size(120, 40);
            newGameButton.ForeColor = Color.White;
            newGameButton.BackColor = Color.Black;
            newGameButton.FlatStyle = FlatStyle.Flat;
            newGameButton.Location = new Point((this.ClientSize.Width - newGameButton.Width) / 2, 120);
            newGameButton.Click += (sender, e) =>
            {
                // Raise the event with the save file name for a new game.
                StartGameRequested?.Invoke(this, "save_0001.json");
            };
            this.Controls.Add(newGameButton);
            
            // Load Game Button
            loadGameButton = new Button();
            loadGameButton.Text = "Load Game";
            loadGameButton.Font = new Font("Arial", 12, FontStyle.Bold);
            loadGameButton.Size = new Size(120, 40);
            loadGameButton.ForeColor = Color.White;
            loadGameButton.BackColor = Color.Black;
            loadGameButton.FlatStyle = FlatStyle.Flat;
            loadGameButton.Location = new Point((this.ClientSize.Width - loadGameButton.Width) / 2, 180);
            loadGameButton.Click += (sender, e) =>
            {
                string userInput = saveFileTextBox.Text.Trim();
                // Default to "save_current.json" if nothing is typed.
                string fileName = string.IsNullOrEmpty(userInput)
                    ? "save_current.json"
                    : (userInput.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? userInput : userInput + ".json");
                // Simply raise the event with the provided file name.
                StartGameRequested?.Invoke(this, fileName);
            };

            this.Controls.Add(loadGameButton);
            
            // TextBox for entering a custom save file name (placed below the Load Game button)
            saveFileTextBox = new TextBox();
            saveFileTextBox.Font = new Font("Arial", 10, FontStyle.Regular);
            saveFileTextBox.Size = new Size(200, 30);
            saveFileTextBox.Location = new Point((this.ClientSize.Width - saveFileTextBox.Width) / 2, loadGameButton.Bottom + 10);
            saveFileTextBox.ForeColor = Color.White;
            saveFileTextBox.BackColor = Color.Black;
            saveFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            saveFileTextBox.PlaceholderText = "Enter save file name (optional)";
            this.Controls.Add(saveFileTextBox);
        }

    }
}
