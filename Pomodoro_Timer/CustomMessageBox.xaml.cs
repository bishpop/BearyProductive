using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string title, string message, bool isSuccess, ConfigData config)
        {
            InitializeComponent();

            // Texte setzen
            TitleText.Text = title;
            MessageText.Text = message;

            // Design anpassen je nach Erfolg oder Fehler
            if (isSuccess)
            {
                TitleText.Text += " 🎉";
                // Standard Button Farbe (Mint) lassen
            }
            else
            {
                TitleText.Text += " ⚠️";
                // Button rot färben bei Fehler
                // (Wir greifen direkt auf den Button im XAML zu, der hat aber keinen Namen, 
                //  deshalb lassen wir es simpel oder du gibst ihm x:Name="OkBtn")
            }

            // Theme Hintergrund setzen (wie in den anderen Fenstern)
            if (config != null && config.ThemeIndex >= 0 && config.ThemeIndex < ThemeManager.PastelThemes.Count)
            {
                MainBorder.Background = ThemeManager.PastelThemes[config.ThemeIndex];
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Signalisiert "OK"
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}