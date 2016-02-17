using System;
using System.Linq;
using System.Windows.Media;
using Telerik.Windows.Controls;
using TelerikOutlookInspiredApp.Appearance;
using TelerikOutlookInspiredApp.Models.Appearance;

namespace TelerikOutlookInspiredApp
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            this.ChangeAppearanceCommand = new DelegateCommand(this.ChangeTheme);

            if (ApplicationThemeViewModel.SelectedTheme == "Office2013")
            {
                TelerikOutlookInspiredApp.Appearance.ThemePalette.Palette.Foreground = Colors.White;
            }
        }

        private void ChangeTheme(object parameter)
        {
            var commandParameter = parameter as AppearanceCommandParameter;
            if (ApplicationThemeViewModel.SelectedTheme == commandParameter.Theme.ToString() &&
                this.ForegroundColorVariation == commandParameter.ColorVariation)
            {
                return;
            }

            this.CurrentTheme = commandParameter.Theme;
            ApplicationThemeViewModel.SelectedTheme = this.CurrentTheme.ToString();

            this.ForegroundColorVariation = commandParameter.ColorVariation;

            AppearanceManager.ChangeAppearance(this.CurrentTheme, this.ForegroundColorVariation);
        }

        public DelegateCommand ChangeAppearanceCommand { get; private set; }

        public ForegroundColorVariations ForegroundColorVariation { get; private set; }

        public Themes CurrentTheme { get; private set; }

    }
}
