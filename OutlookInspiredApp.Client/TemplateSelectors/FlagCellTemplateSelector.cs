using OutlookInspiredApp.Repository.Service;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class FlagCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FlagTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var email = item as Email;
            if (email != null && email.Flag != null)
            {
                return this.FlagTemplate;
            }

            return null;
        }
    }
}
