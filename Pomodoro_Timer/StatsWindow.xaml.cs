using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    public partial class StatsWindow : Window
    {
        private int _totalWork;
        private int _totalBreak;
        private ConfigData _config; // NEU: Config merken für die MessageBox

        public StatsWindow(int totalWorkMinutes, int totalBreakMinutes, ConfigData config)
        {
            InitializeComponent();

            _totalWork = totalWorkMinutes;
            _totalBreak = totalBreakMinutes;
            _config = config; // NEU: Config speichern

            FocusTimeText.Text = $"{_totalWork} min";
            BreakTimeText.Text = $"{_totalBreak} min";

            // Theme anwenden
            if (config != null && config.ThemeIndex >= 0 && config.ThemeIndex < ThemeManager.PastelThemes.Count)
            {
                MainBorder.Background = ThemeManager.PastelThemes[config.ThemeIndex];
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text File (*.txt)|*.txt";
            saveFileDialog.FileName = $"Pomodoro_Session_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt";
            saveFileDialog.Title = "Save Statistics";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string content = "=================================\n";
                    content += "   POMODORO SESSION STATISTICS   \n";
                    content += "=================================\n\n";
                    content += $"Date: {DateTime.Now.ToString("F")}\n\n";
                    content += $"🔥 Total Focus Time: {_totalWork} Minutes\n";
                    content += $"☕ Total Break Time: {_totalBreak} Minutes\n";
                    content += "\nGreat job! Keep it up!\n";
                    content += "=================================";

                    File.WriteAllText(saveFileDialog.FileName, content);

                    // --- HIER GEÄNDERT: CustomMessageBox statt MessageBox ---
                    // true = Erfolg
                    CustomMessageBox msg = new CustomMessageBox("Saved", "Statistics saved successfully!", true, _config);
                    msg.Owner = this; // Damit es mittig über diesem Fenster erscheint
                    msg.ShowDialog();
                }
                catch (Exception ex)
                {
                    // --- HIER AUCH GEÄNDERT ---
                    // false = Fehler
                    CustomMessageBox msg = new CustomMessageBox("Error", "Could not save file:\n" + ex.Message, false, _config);
                    msg.Owner = this;
                    msg.ShowDialog();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
    }
}