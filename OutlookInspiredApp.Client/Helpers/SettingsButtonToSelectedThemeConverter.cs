using System;
using System.Globalization;
using System.Windows.Data;

namespace TelerikOutlookInspiredApp.Helpers
{

    public class SettingsButtonToSelectedThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == ApplicationThemeViewModel.SelectedTheme)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
