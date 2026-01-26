using System;
using System.IO;
using System.Reflection; // WICHTIG für Embedded Resource
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Markup;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private TimeSpan _time;
        private DateTime _endTime;

        private ConfigData _config;
        private bool _isWorkMode = true;
        private double _totalSeconds;
        private int _intervalsLeft;

        private MediaPlayer _player = new MediaPlayer();
        private MiniTimerWindow _miniWindow;

        public MainWindow()
        {
            InitializeComponent();

            _config = ConfigData.Load("config.san");
            _intervalsLeft = _config.TargetIntervals;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;

            ApplyConfig(_config);
            ResetTimer();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            _miniWindow = new MiniTimerWindow(this, _config);
            _miniWindow.Show();
            _miniWindow.UpdateTime(_time.ToString(@"mm\:ss"));
        }

        public void RestoreFromMini()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            _miniWindow = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var remaining = _endTime - DateTime.Now;

            if (remaining.TotalSeconds > 0)
            {
                _time = remaining;
                UpdateDisplay();

                if (_miniWindow != null && _miniWindow.IsVisible)
                {
                    _miniWindow.UpdateTime(_time.ToString(@"mm\:ss"));
                }
            }
            else
            {
                _time = TimeSpan.Zero;
                UpdateDisplay();
                _timer.Stop();

                // HIER WIRD DER SOUND ABGESPIELT
                PlayAlarmSound();

                if (_isWorkMode)
                {
                    _isWorkMode = false;
                    StatusText.Text = "Break Time ☕";
                    TopTitleText.Text = "Bear with me";
                    StartTimerForCurrentMode();
                    Start_Click(null, null);
                }
                else
                {
                    _intervalsLeft--;
                    if (_intervalsLeft > 0)
                    {
                        _isWorkMode = true;
                        StatusText.Text = "Focus Time 🔥";
                        TopTitleText.Text = "Beary Productive";
                        StartTimerForCurrentMode();
                        Start_Click(null, null);
                    }
                    else
                    {
                        if (_miniWindow != null) RestoreFromMini();
                        ShowStatistics();
                        Reset_Click(null, null);
                    }
                }
            }
        }

        private void StartTimerForCurrentMode()
        {
            int minutes = _isWorkMode ? _config.WorkDurationMinutes : _config.BreakDurationMinutes;
            _time = TimeSpan.FromMinutes(minutes);
            _totalSeconds = _time.TotalSeconds;
            TimerProgress.Value = 0;
            UpdateDisplay();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _endTime = DateTime.Now.Add(_time);
            _timer.Start();
        }

        private void Pause_Click(object sender, RoutedEventArgs e) => _timer.Stop();
        private void Reset_Click(object sender, RoutedEventArgs e) => ResetTimer();

        private void ShowStatistics()
        {
            int totalWork = _config.WorkDurationMinutes * _config.TargetIntervals;
            int totalBreak = _config.BreakDurationMinutes * _config.TargetIntervals;

            StatsWindow stats = new StatsWindow(totalWork, totalBreak, _config);
            stats.Owner = this;
            stats.ShowDialog();
        }

        // === NEUE SOUND LOGIK ===
        private void PlayAlarmSound()
        {
            try
            {
                string customPath = _config.SoundPath;
                bool playCustom = false;

                // 1. Prüfen: Hat der User einen Pfad angegeben UND existiert die Datei?
                if (!string.IsNullOrWhiteSpace(customPath) && File.Exists(customPath))
                {
                    playCustom = true;
                }
                else if (!string.IsNullOrWhiteSpace(customPath) && !Path.IsPathRooted(customPath))
                {
                    // Relativen Pfad prüfen (falls im Ordner der .exe)
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, customPath);
                    if (File.Exists(fullPath))
                    {
                        customPath = fullPath;
                        playCustom = true;
                    }
                }

                if (playCustom)
                {
                    // CUSTOM SOUND ABSPIELEN
                    _player.Open(new Uri(customPath));
                    _player.Position = TimeSpan.Zero;
                    _player.Play();
                }
                else
                {
                    // FALLBACK: EINGEBETTETEN SOUND ABSPIELEN
                    PlayEmbeddedSound();
                }
            }
            catch
            {
                // Falls alles schief geht (z.B. Audio-Treiber Fehler), versuchen wir trotzdem den Standard
                try { PlayEmbeddedSound(); } catch { }
            }
        }

        private void PlayEmbeddedSound()
        {
            // Wir erstellen eine temporäre Datei im Temp-Ordner des PCs
            string tempFile = Path.Combine(Path.GetTempPath(), "pomodoro_alarm_temp.mp3");

            // Wenn die Datei dort noch nicht liegt, extrahieren wir sie aus der .exe
            if (!File.Exists(tempFile))
            {
                // HINWEIS: Der Name setzt sich zusammen aus: "Namespace.Ordner.Dateiname"
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Pomodoro_Timer.assets.alarm.mp3";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return;

                    using (FileStream fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            // Jetzt können wir die Temp-Datei abspielen
            _player.Open(new Uri(tempFile));
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }
        // ========================

        private void UpdateDisplay()
        {
            TimerText.Text = _time.ToString(@"mm\:ss");
            if (_totalSeconds > 0)
            {
                double percent = 100 - (_time.TotalSeconds / _totalSeconds * 100);
                if (percent > 100) percent = 100;
                if (percent < 0) percent = 0;
                TimerProgress.Value = percent;
            }
        }

        public void ApplyConfig(ConfigData config)
        {
            _config = config;

            try { if (this.Topmost != config.AlwaysOnTop) this.Topmost = config.AlwaysOnTop; } catch { }

            if (config.ThemeIndex >= 0 && config.ThemeIndex < ThemeManager.PastelThemes.Count)
            {
                MainBorder.Background = ThemeManager.PastelThemes[config.ThemeIndex];
                if (_miniWindow != null && _miniWindow.IsVisible)
                {
                    _miniWindow.MiniBorder.Background = ThemeManager.PastelThemes[config.ThemeIndex];
                }
            }

            try
            {
                FontFamily fontFamily = new FontFamily("Segoe UI");
                if (!string.IsNullOrEmpty(config.SelectedFont))
                {
                    if (config.SelectedFont.Contains("Consolas")) fontFamily = new FontFamily("Consolas");
                    else fontFamily = new FontFamily(config.SelectedFont);
                }
                TimerText.FontFamily = fontFamily;
                StatusText.FontFamily = fontFamily;
                TopTitleText.FontFamily = fontFamily;
            }
            catch { }

            try
            {
                Brush fontBrush = GetBrushFromString(config.FontColor);
                TimerText.Foreground = fontBrush;
                TopTitleText.Foreground = fontBrush;
                StatusText.Foreground = fontBrush;
                TimerProgress.Foreground = GetBrushFromString(config.ProgressColor);
            }
            catch { }
        }

        private Brush GetBrushFromString(string colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString)) return Brushes.White;
            try
            {
                if (colorString.Trim().StartsWith("<")) return (Brush)XamlReader.Parse(colorString);
                else return (Brush)new BrushConverter().ConvertFrom(colorString);
            }
            catch { return Brushes.White; }
        }

        public void UpdateTimerDurations()
        {
            _intervalsLeft = _config.TargetIntervals;
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timer.Stop();
            _isWorkMode = true;
            _intervalsLeft = _config.TargetIntervals;
            StatusText.Text = "Focus Time 🎯";
            TopTitleText.Text = "Beary Productive";
            StartTimerForCurrentMode();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow opt = new OptionsWindow(this, _config);
            opt.Owner = this;
            opt.ShowDialog();
        }
    }
}