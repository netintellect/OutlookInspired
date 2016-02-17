using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace TelerikOutlookInspiredApp
{
    /// <summary>
    /// Interaction logic for MailView.xaml
    /// </summary>
    public partial class MailView : UserControl
    {
        private FilterDescriptor unreadDescriptor;

        public MailView()
        {
            InitializeComponent();
            this.InitializeUnreadFilterDescriptor();
        }

        private void InitializeUnreadFilterDescriptor()
        {
            this.unreadDescriptor = new FilterDescriptor
            {
                Member = "Status",
                Operator = FilterOperator.IsEqualTo,
                Value = EmailStatus.Unread,
            };
        }

        private void OnAllRadioButtonClick(object sender, RoutedEventArgs e)
        {
            this.gridView.FilterDescriptors.Remove(this.unreadDescriptor);
        }

        private void OnUnreadRadioButtonClick(object sender, RoutedEventArgs e)
        {
            this.gridView.FilterDescriptors.Add(this.unreadDescriptor);
        }
    }
}
