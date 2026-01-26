using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

namespace PomodoroTimer
{
    public partial class OptionsWindow : Window
    {
        private MainWindow _mainWindow;
        private ConfigData _tempConfig;

        public OptionsWindow(MainWindow main, ConfigData currentConfig)
        {
            InitializeComponent();
            _mainWindow = main;
            _tempConfig = currentConfig;

            ThemeItemsControl.ItemsSource = ThemeManager.PastelThemes;

            WorkTimeBox.Text = _tempConfig.WorkDurationMinutes.ToString();
            BreakTimeBox.Text = _tempConfig.BreakDurationMinutes.ToString();
            IntervalBox.Text = _tempConfig.TargetIntervals.ToString();
            SoundPathBox.Text = _tempConfig.SoundPath;

            foreach (ComboBoxItem item in FontComboBox.Items)
            {
                if (item.Content.ToString() == _tempConfig.SelectedFont)
                {
                    FontComboBox.SelectedItem = item;
                    break;
                }
            }

            AlwaysOnTopCheckBox.IsChecked = _tempConfig.AlwaysOnTop;

            if (_tempConfig.ThemeIndex >= 0 && _tempConfig.ThemeIndex < ThemeManager.PastelThemes.Count)
            {
                MainBorder.Background = ThemeManager.PastelThemes[_tempConfig.ThemeIndex];
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void BrowseSound_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.wma|All Files|*.*";
            openFileDialog.Title = "Select Alarm Sound";

            if (openFileDialog.ShowDialog() == true)
            {
                _tempConfig.SoundPath = openFileDialog.FileName;
                SoundPathBox.Text = openFileDialog.FileName;
            }
        }

        // --- SICHERES UPDATE ---
        private void AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            if (AlwaysOnTopCheckBox.IsChecked.HasValue)
            {
                // Wir speichern den Wert NUR in der Config-Variable.
                _tempConfig.AlwaysOnTop = AlwaysOnTopCheckBox.IsChecked.Value;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WorkTimeBox.Text, out int work)) _tempConfig.WorkDurationMinutes = work;
            if (int.TryParse(BreakTimeBox.Text, out int breakTime)) _tempConfig.BreakDurationMinutes = breakTime;
            if (int.TryParse(IntervalBox.Text, out int intervals)) _tempConfig.TargetIntervals = intervals;

            _tempConfig.AlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;

            ConfigData.Save(_tempConfig, "config.san");

            // DIESE ZEILE HAT GEFEHLT: Die Änderungen (inkl. TopMost) anwenden
            _mainWindow.ApplyConfig(_tempConfig);

            _mainWindow.UpdateTimerDurations();
            this.Close();
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Background is Brush brush)
            {
                if (btn.DataContext is Brush originalBrush)
                {
                    _tempConfig.ThemeIndex = ThemeManager.PastelThemes.IndexOf(originalBrush);
                    _mainWindow.ApplyConfig(_tempConfig);
                    MainBorder.Background = originalBrush;
                }
            }
        }

        private void FontColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Background is Brush brush)
            {
                _tempConfig.FontColor = XamlWriter.Save(brush);
                _mainWindow.ApplyConfig(_tempConfig);
            }
        }

        private void ProgressColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Background is Brush brush)
            {
                _tempConfig.ProgressColor = XamlWriter.Save(brush);
                _mainWindow.ApplyConfig(_tempConfig);
            }
        }

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontComboBox.SelectedItem is ComboBoxItem item)
            {
                _tempConfig.SelectedFont = item.Content.ToString();
                _mainWindow.ApplyConfig(_tempConfig);
            }
        }
    }
}