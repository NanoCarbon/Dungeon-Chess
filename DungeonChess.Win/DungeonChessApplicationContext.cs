using System;
using System.Windows.Forms;

namespace DungeonChess.Win
{
    public class DungeonChessApplicationContext : ApplicationContext
    {
        // Implementation goes here.
        public DungeonChessApplicationContext()
        {
            ShowStartForm();
        }

        private void ShowStartForm()
        {
            var startForm = new StartForm();
            startForm.StartGameRequested += (sender, saveFileName) =>
            {
                // Hide the start form immediately.
                startForm.Hide();
                // Use startForm.BeginInvoke to delay the closure until after the MainForm is shown.
                startForm.BeginInvoke(new Action(() =>
                {
                    ShowMainForm(saveFileName);
                    startForm.Close();
                }));
            };
            startForm.FormClosed += OnFormClosed;
            startForm.Show();
        }

        private void ShowMainForm(string saveFileName)
        {
            var mainForm = new MainForm(saveFileName);
            mainForm.GameOver += (sender, winningPlayer) =>
            {
                // Retrieve the serialized board state from MainForm.
                string boardStateJson = mainForm.GetBoardStateJson();
                // Create the GameOverForm with both winningPlayer and boardStateJson.
                using (var gameOverForm = new GameOverForm(winningPlayer, boardStateJson))
                {
                    gameOverForm.ShowDialog(mainForm);
                }
                ShowStartForm();
                mainForm.Close();
            };
            mainForm.FormClosed += OnFormClosed;
            mainForm.Show();
        }



        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
                ExitThread();
        }
    }
}
