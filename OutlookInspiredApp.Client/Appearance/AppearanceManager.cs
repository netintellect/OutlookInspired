using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using TelerikOutlookInspiredApp.Analytics;
using TelerikOutlookInspiredApp.Models.Appearance;

namespace TelerikOutlookInspiredApp.Appearance
{
    public static class AppearanceManager
    {
        private static readonly string telerikThemePrefix = "/Telerik.Windows.Themes.";
        private static readonly string telerikThemeAssemblyFolder = ";component/Themes/";
        private static readonly string office2013Theme = "Office2013";
        private static readonly string visualStudio2013Theme = "VisualStudio2013";

        public static void ChangeAppearance(Themes theme, ForegroundColorVariations foregroundColorVariation)
        {
            string analyticsFeatureName = string.Concat(EqatecConstants.Theme, ".", theme.ToString());
            EqatecMonitor.Instance.TrackFeature(analyticsFeatureName);

            switch (foregroundColorVariation)
                {
                    case ForegroundColorVariations.Light:
                        ThemePalette.Palette.Foreground = ThemePalette.LightForegroundColor;
                        break;
                    case ForegroundColorVariations.Dark:
                        ThemePalette.Palette.Foreground = ThemePalette.DarkForegroundColor;
                        break;
                    case ForegroundColorVariations.ExpressionDark:
                        ThemePalette.Palette.Foreground = ThemePalette.ExpressionDarkForegroundColor;
                        break;
                    case ForegroundColorVariations.Windows7:
                        ThemePalette.Palette.Foreground = ThemePalette.Windows7ForegroundColor;
                        break;
                    default:
                        break;
                }

            string themeAssembly = GetThemeAssemblyName(theme.ToString());
            SetTheme(themeAssembly);
            SetThemeSpecific(theme);
            SetIconsSet(theme);

            OnThemeChanged(theme, foregroundColorVariation);
        }

        private static void SetThemeSpecific(Themes theme)
        {
            switch (theme)
            {
                case Themes.Office2013:
                    Office2013Palette.LoadPreset(Office2013Palette.ColorVariation.White);
                    ApplicationThemeViewModel.SelectedTheme = "Office2013";
                    break;
                case Themes.VisualStudio2013:
                    VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Light);
                    ApplicationThemeViewModel.SelectedTheme = "VisualStudio2013";
                    break;
                case Themes.VisualStudio2013_Blue:
                    VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Blue);
                    ApplicationThemeViewModel.SelectedTheme = "VisualStudio2013_Blue";
                    break;
                case Themes.VisualStudio2013_Dark:
                    VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
                    ApplicationThemeViewModel.SelectedTheme = "VisualStudio2013_Dark";
                    break;
                default:
                    break;
            }
        }

		private static void SetIconsSet(Themes theme)
		{
			switch (theme)
			{
				case Themes.Office_Blue:
				case Themes.Summer:
				case Themes.Vista:
				case Themes.Windows7:
					IconSources.ChangeIconsSet(IconsSet.Light);
					break;
				case Themes.Expression_Dark:
				case Themes.VisualStudio2013_Dark:
					IconSources.ChangeIconsSet(IconsSet.Dark);
					break;
				default:
					IconSources.ChangeIconsSet(IconsSet.Modern);
					break;
			}
		}

        private static string GetThemeAssemblyName(string theme)
        {
            if (theme.Contains(office2013Theme))
            {
                return office2013Theme;
            }
            else if (theme.Contains(visualStudio2013Theme))
            {
                return visualStudio2013Theme;
            }
            else
            {
                return theme;
            }
        }

        private static void SetTheme(string theme)
        {
            ApplicationThemeViewModel.SelectedTheme = theme;

            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "System.Windows.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Documents.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.GridView.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.ImageEditor.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.Input.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.Navigation.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.RibbonView.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.RichTextBoxUI.xaml"), UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(string.Concat(telerikThemePrefix, theme, telerikThemeAssemblyFolder, "Telerik.Windows.Controls.ScheduleView.xaml"), UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/Brushes.xaml", UriKind.RelativeOrAbsolute)
            });


            if (theme != "Expression_Dark")
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/CalendarStyle.xaml", UriKind.RelativeOrAbsolute)
                });
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/CalendarStyleExpressionDark.xaml", UriKind.RelativeOrAbsolute)
                });
            }

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/GridViewStyle.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/OutlookBarStyle.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/RibbonWindowStyle.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/RichTextBoxRibbonUIStyle.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/ScheduleViewStyle.xaml", UriKind.RelativeOrAbsolute)
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/TreeViewStyle.xaml", UriKind.RelativeOrAbsolute)
            });

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/TelerikOutlookInspiredApp;component/Styles/CommonStyles.xaml", UriKind.RelativeOrAbsolute)
            });
        }

        public static event EventHandler<AppearanceChangedEventArgs> ThemeChanged;
        private static void OnThemeChanged(Themes theme, ForegroundColorVariations colorVariation)
        {
            if (ThemeChanged != null)
            {
                ThemeChanged(null, new AppearanceChangedEventArgs(theme, colorVariation));
            }
        }
    }
}
