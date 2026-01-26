using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PomodoroTimer
{
    public partial class MiniTimerWindow : Window
    {
        private MainWindow _mainWindow;

        public MiniTimerWindow(MainWindow main, ConfigData config)
        {
            InitializeComponent();
            _mainWindow = main;

            // Design übernehmen
            if (config.ThemeIndex >= 0 && config.ThemeIndex < ThemeManager.PastelThemes.Count)
            {
                MiniBorder.Background = ThemeManager.PastelThemes[config.ThemeIndex];
            }

            // Schriftart übernehmen (optional)
            try { MiniTimeText.FontFamily = new FontFamily(config.SelectedFont); } catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Position: Unten Rechts über der Taskleiste
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 20; // 20px von rechts
            this.Top = desktopWorkingArea.Bottom - this.Height - 20; // 20px von unten
        }

        public void UpdateTime(string timeText)
        {
            MiniTimeText.Text = timeText;
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.RestoreFromMini();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}