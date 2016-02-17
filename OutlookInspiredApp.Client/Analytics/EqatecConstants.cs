using System;
using System.Linq;

namespace TelerikOutlookInspiredApp.Analytics
{
    public static class EqatecConstants
    {
        // crashes
        public const string ApplicationCrash = "Application.Crash";

        // features to get time
        public const string ApplicationStartupTime = "Application.StartUpTime"; // the time between the app launch and the home view loaded
        public const string ApplicationUptime = "Application.UpTime";

        // themes to track
        public const string Theme = "Theme";

        // views to track
        public const string CalendarViewUpTime = "CalendarView";
        public const string MailViewUpTime = "MailView";
    }
}