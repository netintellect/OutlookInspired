using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TelerikOutlookInspiredApp.Helpers
{
	class CategoryToBrushConverter : IValueConverter
	{
		string categoryName;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			else if (value is CalendarType)
			{
				categoryName = (value as CalendarType).Name;
			}
			else
			{
				categoryName = value.ToString();
			}
			
			switch (categoryName)
			{
				case "Company":
                    return new SolidColorBrush(Color.FromArgb(255, 165, 224, 233));                    
				case "Team":
                    return new SolidColorBrush(Color.FromArgb(255, 223, 243, 187));
				default:
                    return new SolidColorBrush(Color.FromArgb(255, 247, 199, 165));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
