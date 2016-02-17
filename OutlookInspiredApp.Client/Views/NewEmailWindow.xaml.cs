using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    /// <summary>
    /// Interaction logic for NewEmailWindow.xaml
    /// </summary>
    public partial class NewEmailWindow : RadRibbonWindow
    {
        public NewEmailWindow()
        {
            InitializeComponent();
        }

        public NewEmailWindow(MailViewModel viewModel) 
            : this()
        {
            this.DataContext = viewModel;
        }
    }
}