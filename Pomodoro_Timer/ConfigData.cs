using System;
using System.IO;
using System.Xml.Serialization;

namespace PomodoroTimer
{
    public class ConfigData
    {
        // Standard Werte
        public int WorkDurationMinutes { get; set; } = 25;
        public int BreakDurationMinutes { get; set; } = 5;
        public int TargetIntervals { get; set; } = 4;
        public string SoundPath { get; set; } = "";

        // Design Werte
        public int ThemeIndex { get; set; } = 0;
        public string SelectedFont { get; set; } = "Segoe UI";
        public string FontColor { get; set; } = "#FFFFFF";
        public string ProgressColor { get; set; } = "#FFFFFF";

        // --- NEU: Always On Top Einstellung ---
        public bool AlwaysOnTop { get; set; } = false;

        // Speichern & Laden
        public static void Save(ConfigData data, string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
                using (TextWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, data);
                }
            }
            catch { }
        }

        public static ConfigData Load(string filename)
        {
            if (!File.Exists(filename)) return new ConfigData();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigData));
                using (TextReader reader = new StreamReader(filename))
                {
                    return (ConfigData)serializer.Deserialize(reader);
                }
            }
            catch { return new ConfigData(); }
        }
    }
}