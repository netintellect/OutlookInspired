using System.Windows;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadRibbonWindow
    {
        static MainWindow()
        {
            RadRibbonWindow.IsWindowsThemeEnabled = false;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IconSources.ChangeIconsSet(IconsSet.Modern); 
        }
    }
}
