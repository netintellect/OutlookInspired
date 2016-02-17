using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace TelerikOutlookInspiredApp.Appearance
{
    public sealed class ThemePalette : Freezable
    {
        private struct PropertyNames
        {
            public const string Foreground = "Foreground";
        }

        private static readonly Color defaultDarkForegroundColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);

        private static readonly Color defaultLightForegroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

        private static readonly Color defaultExpressionDarkForegroundColor = Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD);

        private static readonly Color defaultWindows7ForegroundColor = Color.FromArgb(0xFF, 0x00, 0x6E, 0x12);

        private static readonly ThemePalette palette = new ThemePalette();

        static ThemePalette()
        {
            Initialize();
        }

        public static ThemePalette Palette
        {
            get
            {
                return ThemePalette.palette;
            }
        }

        internal static Color LightForegroundColor
        {
            get
            {
                return defaultLightForegroundColor;
            }
        }
      
        internal static Color DarkForegroundColor
        {
            get
            {
                return defaultDarkForegroundColor;
            }
        }

        internal static Color ExpressionDarkForegroundColor
        {
            get
            {
                return defaultExpressionDarkForegroundColor;
            }
        }
       

        internal static Color Windows7ForegroundColor
        {
            get
            {
                return defaultWindows7ForegroundColor;
            }
        }

        public static readonly DependencyProperty ForegroundProperty =
          DependencyProperty.Register("Foreground", typeof(Color), typeof(ThemePalette), new PropertyMetadata());

        public Color Foreground
        {
            get { return (Color)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        private static void Initialize()
        {
            palette.Foreground = defaultLightForegroundColor;
        }

        internal static bool TryGetResource(Resources key, out string resource)
        {
            bool containsResource = false;
            switch (key)
            {
                case Resources.Foreground:
                    resource = PropertyNames.Foreground;
                    containsResource = true;
                    break;
                default:
                    resource = string.Empty;
                    break;
            }

            return containsResource;
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
