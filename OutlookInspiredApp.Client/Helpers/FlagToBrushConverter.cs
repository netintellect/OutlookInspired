using System;
using System.Linq;
using System.Windows.Data;

namespace TelerikOutlookInspiredApp
{
    public class FlagToBrushConverter : IValueConverter
    {
        private readonly string todayBrush = "#FFD76443";
        private readonly string tomorrowBrush = "#FFE3937C";
        private readonly string weekBrush = "#FFF3D0C6";
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var flag = (FollowUpType)value;

            if (flag == FollowUpType.Today || flag == FollowUpType.NoDate)
            {
                return this.todayBrush;
            }
            else if (flag == FollowUpType.Tomorrow)
            {
                return this.tomorrowBrush;
            }
            else
            {
                return this.weekBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
