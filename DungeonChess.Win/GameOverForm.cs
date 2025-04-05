using System;
using System.Drawing;
using System.Windows.Forms;

namespace DungeonChess.Win
{
    public class GameOverForm : Form
    {
        public GameOverForm(int winningPlayer)
        {
            this.Text = "Game Over";
            this.Size = new Size(300, 200);
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

            // Return button.
            Button returnButton = new Button();
            returnButton.Text = "Return to Start";
            returnButton.Font = new Font("Arial", 12, FontStyle.Bold);
            returnButton.ForeColor = Color.White;
            returnButton.BackColor = Color.Gray;
            returnButton.Size = new Size(150, 40);
            returnButton.Location = new Point((this.ClientSize.Width - returnButton.Width) / 2, message.Bottom + 10);
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
