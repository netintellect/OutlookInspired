using System;
using System.Linq;
using Telerik.Windows.Controls;
using TelerikOutlookInspiredApp.Analytics;

namespace TelerikOutlookInspiredApp
{
    public partial class MainViewModel
    {
        public DelegateCommand OutlookBarSelectionChangedCommand { get; private set; }

        private void InitializeCommands()
        {
            this.OutlookBarSelectionChangedCommand = new DelegateCommand(this.OnOutlookBarSelectionChangedCommandExecuted);
        }

        private void OnOutlookBarSelectionChangedCommandExecuted(object parameter)
        {
            this.StopTrackingCurrentView();

            this.SelectedViewModel = this.SelectedOutlookSection.ViewModel;

            this.TrackCurrentView();
        }
        
        public void StopTrackingCurrentView()
        {
            var currentView = this.SelectedViewModel is MailViewModel ? EqatecConstants.MailViewUpTime : EqatecConstants.CalendarViewUpTime;
            EqatecMonitor.Instance.TrackFeatureStop(currentView);
        }

        private void TrackCurrentView()
        {
            var currentView = this.SelectedViewModel is MailViewModel ? EqatecConstants.MailViewUpTime : EqatecConstants.CalendarViewUpTime;
            EqatecMonitor.Instance.TrackFeatureStart(currentView);
        }
    }
}
