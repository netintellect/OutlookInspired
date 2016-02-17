using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class OutlookBarTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CalendarOutlookSectionTemplate { get; set; }
        public DataTemplate MailOutlookSectionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var outlookSection = item as OutlookSection;
            if (outlookSection != null)
            {
                var name = outlookSection.Name;
                switch (name)
                {
                    case "Mail":
                        return this.MailOutlookSectionTemplate;
                    case "Calendar":
                        return this.CalendarOutlookSectionTemplate;
                }
            }

            return null;
        }
    }
}
