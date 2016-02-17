using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using TelerikOutlookInspiredApp.Analytics;

namespace TelerikOutlookInspiredApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const string LogFilename = "outlookInspiredApp.log";
        public App()
        {
            ApplicationThemeViewModel.SelectedTheme = "Office2013";

            this.Startup += this.OnApplicationStartup;
            this.LoadCompleted += this.OnLoadCompleted;
            this.Exit += this.OnApplicationExit; 
            this.DispatcherUnhandledException += this.Application_DispatcherUnhandledException;

            InitializeComponent();
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            EqatecMonitor.TryInitializeMonitor();
            EqatecMonitor.Instance.TrackFeatureStart(EqatecConstants.ApplicationStartupTime);
            EqatecMonitor.Instance.TrackFeatureStart(EqatecConstants.ApplicationUptime);         
        }

        private void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            EqatecMonitor.Instance.TrackFeatureStop(EqatecConstants.ApplicationStartupTime);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            EqatecMonitor.Instance.TrackFeatureStop(EqatecConstants.ApplicationUptime);
            EqatecMonitor.Stop();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            /* More detailed error message display */
            string exception = string.Empty;
            Exception ex = e.Exception;
            while (ex != null)
            {
                exception += String.Format("----------------\n{0}\n", ex.Message);
                exception += String.Format("{0}\n", ex.StackTrace);
                ex = ex.InnerException;
            }

            string logExceptionHeader = string.Format("Exception at {0}.{1} :", DateTime.Now, DateTime.Now.Millisecond);
            string logExceptionFooter = "End of exception.";
            string logText = string.Join(Environment.NewLine, logExceptionHeader, exception, logExceptionFooter, Environment.NewLine);
            File.AppendAllText(LogFilename, logText);

            // EQATEC: cancel application startup timing if ongoing, report app crash and exception
            EqatecMonitor.Instance.TrackFeatureCancel(EqatecConstants.ApplicationStartupTime);
            EqatecMonitor.Instance.TrackFeatureCancel(EqatecConstants.ApplicationUptime);
            EqatecMonitor.Instance.TrackFeature(EqatecConstants.ApplicationCrash);
            EqatecMonitor.Instance.TrackException(e.Exception, logText);
            EqatecMonitor.Stop();
        }
    }
}
