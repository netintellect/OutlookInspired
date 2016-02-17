using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class SectorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CalendarTemplate { get; set; }

        public DataTemplate MailTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is MailViewModel ? this.MailTemplate : this.CalendarTemplate;
        }
    }
}
