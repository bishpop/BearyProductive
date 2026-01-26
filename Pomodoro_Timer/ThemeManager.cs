using System.Collections.Generic;
using System.Windows.Media;

namespace PomodoroTimer
{
    public static class ThemeManager
    {
        public static List<Brush> PastelThemes { get; private set; }

        static ThemeManager()
        {
            var converter = new BrushConverter();
            PastelThemes = new List<Brush>();

            // --- Einfache Farben (Solid) ---
            PastelThemes.Add((Brush)converter.ConvertFrom("#FFB7B2")); // 1. Rot/Pink
            PastelThemes.Add((Brush)converter.ConvertFrom("#FFDAC1")); // 2. Pfirsich
            PastelThemes.Add((Brush)converter.ConvertFrom("#FFF2CC")); // 3. Gelb
            PastelThemes.Add((Brush)converter.ConvertFrom("#B5EAD7")); // 4. Mint
            PastelThemes.Add((Brush)converter.ConvertFrom("#C7CEEA")); // 5. Lila-Blau
            PastelThemes.Add((Brush)converter.ConvertFrom("#E2F0CB")); // 6. Limette
            PastelThemes.Add((Brush)converter.ConvertFrom("#FF9AA2")); // 7. Lachs
            PastelThemes.Add((Brush)converter.ConvertFrom("#1E1E2E")); // 8. Dark Mode (Solid)

            // --- Die Kawaii Gradients ---

            // 9. "Cotton Candy" (Rosa -> Hellblau)
            PastelThemes.Add(CreateGradient("#FF9A9E", "#FECFEF", 45));

            // 10. "Magic Mint" (Mint -> Türkis)
            PastelThemes.Add(CreateGradient("#84FAB0", "#8FD3F4", 45));

            // 11. "Peach Sunset" (Pfirsich -> Gelb)
            PastelThemes.Add(CreateGradient("#FBAB7E", "#F7CE68", 45));

            // --- NEU: Die Dark Gradients ---

            // 12. "Catppuccin Vibe" (#1E1E2E -> Soft Lavender/Grey)
            // Startet mit deiner Wunschfarbe und wird sanft etwas heller/lilastichiger
            PastelThemes.Add(CreateGradient("#1E1E2E", "#3E3B54", 45));

            // 13. "Royal Blue" (Royal Blue -> Deep Navy)
            // Kräftig, edel und glänzend
            PastelThemes.Add(CreateGradient("#4b6cb7", "#182848", 45));

            // 14. "Midnight Calm" (Deep Night -> Calm Blue/Teal)
            // Sehr beruhigend, wie der Nachthimmel
            PastelThemes.Add(CreateGradient("#0F2027", "#2C5364", 45));
        }

        // Hilfsmethode für Verläufe
        private static LinearGradientBrush CreateGradient(string color1, string color2, double angle)
        {
            var brush = new LinearGradientBrush();
            brush.StartPoint = new System.Windows.Point(0, 0);
            brush.EndPoint = new System.Windows.Point(1, 1);

            var c1 = (Color)ColorConverter.ConvertFromString(color1);
            var c2 = (Color)ColorConverter.ConvertFromString(color2);

            brush.GradientStops.Add(new GradientStop(c1, 0.0));
            brush.GradientStops.Add(new GradientStop(c2, 1.0));

            return brush;
        }
    }
}