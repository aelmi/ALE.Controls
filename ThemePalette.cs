using System.Drawing;

namespace ALE.Controls.Theming
{
    public static class ThemePalette
    {
        public static ThemeColors GetPalette(GridTheme theme)
        {
            return theme switch
            {
                GridTheme.None => GetSystemPalette(),

                GridTheme.Clean => new ThemeColors
                {
                    GridBackground = Color.White,
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(249, 250, 251),
                    // This is the light hover color
                    RowHover = Color.FromArgb(235, 240, 255),
                    HeaderBackground = Color.FromArgb(249, 250, 251),
                    HeaderText = Color.FromArgb(55, 65, 81),
                    GridLines = Color.FromArgb(243, 244, 246),
                    TextPrimary = Color.FromArgb(55, 65, 81),
                    ToolbarBackground = Color.FromArgb(249, 250, 251),
                    Accent = Color.FromArgb(99, 102, 241),
                    // Updated to the Light Blue Selection from your EUM screenshot
                    SelectionBackground = Color.FromArgb(215, 225, 250),
                    SelectionForeground = Color.FromArgb(30, 40, 60)
                },

                GridTheme.Dark => new ThemeColors
                {
                    GridBackground = Color.FromArgb(30, 30, 30),
                    RowEven = Color.FromArgb(35, 35, 35),
                    RowOdd = Color.FromArgb(42, 42, 42),
                    RowHover = Color.FromArgb(50, 50, 50),
                    HeaderBackground = Color.FromArgb(55, 65, 81),
                    HeaderText = Color.FromArgb(230, 230, 230),
                    GridLines = Color.FromArgb(60, 60, 60),
                    TextPrimary = Color.FromArgb(220, 220, 220),
                    ToolbarBackground = Color.FromArgb(25, 25, 25),
                    Accent = Color.FromArgb(79, 70, 229),
                    SelectionBackground = Color.FromArgb(79, 70, 229),
                    SelectionForeground = Color.White
                },

                GridTheme.Enterprise => new ThemeColors
                {
                    GridBackground = Color.White,
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(250, 250, 250),
                    RowHover = Color.FromArgb(228, 244, 255),
                    HeaderBackground = Color.FromArgb(240, 240, 240),
                    HeaderText = Color.FromArgb(0, 0, 0),
                    GridLines = Color.FromArgb(204, 204, 204),
                    TextPrimary = Color.FromArgb(0, 0, 0),
                    ToolbarBackground = Color.FromArgb(240, 240, 240),
                    Accent = Color.FromArgb(0, 120, 215),
                    SelectionBackground = SystemColors.Highlight,
                    SelectionForeground = SystemColors.HighlightText
                },

                GridTheme.Blue => new ThemeColors
                {
                    GridBackground = Color.FromArgb(248, 250, 252),
                    RowEven = Color.FromArgb(248, 250, 252),
                    RowOdd = Color.FromArgb(241, 245, 249),
                    RowHover = Color.FromArgb(226, 232, 240),
                    HeaderBackground = Color.FromArgb(37, 99, 235),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(203, 213, 225),
                    TextPrimary = Color.FromArgb(30, 41, 59),
                    ToolbarBackground = Color.FromArgb(241, 245, 249),
                    Accent = Color.FromArgb(59, 130, 246),
                    SelectionBackground = Color.FromArgb(37, 99, 235),
                    SelectionForeground = Color.White
                },

                GridTheme.Green => new ThemeColors
                {
                    GridBackground = Color.FromArgb(243, 248, 244),
                    RowEven = Color.FromArgb(255, 255, 255),
                    RowOdd = Color.FromArgb(240, 253, 244),
                    RowHover = Color.FromArgb(220, 252, 231),
                    HeaderBackground = Color.FromArgb(34, 139, 34),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(187, 247, 208),
                    TextPrimary = Color.FromArgb(21, 87, 36),
                    ToolbarBackground = Color.FromArgb(240, 253, 244),
                    Accent = Color.FromArgb(22, 163, 74),
                    SelectionBackground = Color.FromArgb(34, 139, 34),
                    SelectionForeground = Color.White
                },

                GridTheme.Grey => new ThemeColors
                {
                    GridBackground = Color.FromArgb(250, 250, 250),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(245, 245, 245),
                    RowHover = Color.FromArgb(235, 235, 235),
                    HeaderBackground = Color.FromArgb(75, 85, 99),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(220, 220, 220),
                    TextPrimary = Color.FromArgb(31, 41, 55),
                    ToolbarBackground = Color.FromArgb(245, 245, 245),
                    Accent = Color.FromArgb(107, 114, 128),
                    SelectionBackground = Color.FromArgb(75, 85, 99),
                    SelectionForeground = Color.White
                },

                GridTheme.Pink => new ThemeColors
                {
                    GridBackground = Color.FromArgb(253, 242, 248),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(252, 231, 243),
                    RowHover = Color.FromArgb(251, 207, 232),
                    HeaderBackground = Color.FromArgb(219, 39, 119),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(244, 194, 194),
                    TextPrimary = Color.FromArgb(131, 24, 67),
                    ToolbarBackground = Color.FromArgb(252, 231, 243),
                    Accent = Color.FromArgb(236, 72, 153),
                    SelectionBackground = Color.FromArgb(219, 39, 119),
                    SelectionForeground = Color.White
                },

                GridTheme.DarkBlue => new ThemeColors
                {
                    GridBackground = Color.FromArgb(30, 41, 59),
                    RowEven = Color.FromArgb(30, 41, 59),
                    RowOdd = Color.FromArgb(51, 65, 85),
                    RowHover = Color.FromArgb(71, 85, 105),
                    HeaderBackground = Color.FromArgb(30, 58, 138),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(71, 85, 105),
                    TextPrimary = Color.FromArgb(226, 232, 240),
                    ToolbarBackground = Color.FromArgb(15, 23, 42),
                    Accent = Color.FromArgb(96, 165, 250),
                    SelectionBackground = Color.FromArgb(30, 58, 138),
                    SelectionForeground = Color.White
                },

                GridTheme.NavyBlue => new ThemeColors
                {
                    GridBackground = Color.FromArgb(238, 243, 250),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(231, 239, 253),
                    RowHover = Color.FromArgb(191, 219, 254),
                    HeaderBackground = Color.FromArgb(0, 0, 128),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(147, 197, 253),
                    TextPrimary = Color.FromArgb(30, 64, 175),
                    ToolbarBackground = Color.FromArgb(231, 239, 253),
                    Accent = Color.FromArgb(29, 78, 216),
                    SelectionBackground = Color.FromArgb(0, 0, 128),
                    SelectionForeground = Color.White
                },

                GridTheme.Orange => new ThemeColors
                {
                    GridBackground = Color.FromArgb(255, 247, 237),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(255, 237, 213),
                    RowHover = Color.FromArgb(254, 215, 170),
                    HeaderBackground = Color.FromArgb(234, 88, 12),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(253, 186, 116),
                    TextPrimary = Color.FromArgb(124, 45, 18),
                    ToolbarBackground = Color.FromArgb(255, 237, 213),
                    Accent = Color.FromArgb(249, 115, 22),
                    SelectionBackground = Color.FromArgb(234, 88, 12),
                    SelectionForeground = Color.White
                },

                GridTheme.Purple => new ThemeColors
                {
                    GridBackground = Color.FromArgb(250, 245, 255),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(243, 232, 255),
                    RowHover = Color.FromArgb(221, 214, 254),
                    HeaderBackground = Color.FromArgb(126, 34, 206),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(196, 181, 253),
                    TextPrimary = Color.FromArgb(88, 28, 135),
                    ToolbarBackground = Color.FromArgb(243, 232, 255),
                    Accent = Color.FromArgb(168, 85, 247),
                    SelectionBackground = Color.FromArgb(126, 34, 206),
                    SelectionForeground = Color.White
                },

                GridTheme.Teal => new ThemeColors
                {
                    GridBackground = Color.FromArgb(240, 253, 250),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(204, 251, 241),
                    RowHover = Color.FromArgb(153, 246, 228),
                    HeaderBackground = Color.FromArgb(13, 148, 136),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(153, 246, 228),
                    TextPrimary = Color.FromArgb(19, 78, 74),
                    ToolbarBackground = Color.FromArgb(204, 251, 241),
                    Accent = Color.FromArgb(20, 184, 166),
                    SelectionBackground = Color.FromArgb(13, 148, 136),
                    SelectionForeground = Color.White
                },

                GridTheme.Red => new ThemeColors
                {
                    GridBackground = Color.FromArgb(254, 242, 242),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(254, 226, 226),
                    RowHover = Color.FromArgb(254, 202, 202),
                    HeaderBackground = Color.FromArgb(220, 38, 38),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(252, 165, 165),
                    TextPrimary = Color.FromArgb(127, 29, 29),
                    ToolbarBackground = Color.FromArgb(254, 226, 226),
                    Accent = Color.FromArgb(239, 68, 68),
                    SelectionBackground = Color.FromArgb(220, 38, 38),
                    SelectionForeground = Color.White
                },

                GridTheme.Indigo => new ThemeColors
                {
                    GridBackground = Color.FromArgb(238, 242, 255),
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(224, 231, 255),
                    RowHover = Color.FromArgb(199, 210, 254),
                    HeaderBackground = Color.FromArgb(79, 70, 229),
                    HeaderText = Color.White,
                    GridLines = Color.FromArgb(165, 180, 252),
                    TextPrimary = Color.FromArgb(55, 48, 163),
                    ToolbarBackground = Color.FromArgb(224, 231, 255),
                    Accent = Color.FromArgb(99, 102, 241),
                    SelectionBackground = Color.FromArgb(79, 70, 229),
                    SelectionForeground = Color.White
                },

                _ => new ThemeColors
                {
                    GridBackground = Color.White,
                    RowEven = Color.White,
                    RowOdd = Color.FromArgb(249, 250, 251),
                    RowHover = Color.FromArgb(243, 244, 246),
                    HeaderBackground = Color.FromArgb(243, 244, 246),
                    HeaderText = Color.FromArgb(17, 24, 39),
                    GridLines = Color.FromArgb(229, 231, 235),
                    TextPrimary = Color.FromArgb(17, 24, 39),
                    ToolbarBackground = Color.FromArgb(249, 250, 251),
                    Accent = Color.FromArgb(79, 70, 229),
                    SelectionBackground = Color.FromArgb(215, 225, 250),
                    SelectionForeground = Color.FromArgb(30, 40, 60)
                }
            };
        }

        private static ThemeColors GetSystemPalette()
        {
            return new ThemeColors
            {
                GridBackground = SystemColors.Window,
                RowEven = SystemColors.Window,
                RowOdd = SystemColors.Window,
                RowHover = SystemColors.Highlight,
                HeaderBackground = SystemColors.Control,
                HeaderText = SystemColors.WindowText,
                GridLines = SystemColors.ControlDark,
                TextPrimary = SystemColors.WindowText,
                ToolbarBackground = SystemColors.Control,
                Accent = SystemColors.Highlight,
                SelectionBackground = SystemColors.Highlight,
                SelectionForeground = SystemColors.HighlightText
            };
        }
    }
}