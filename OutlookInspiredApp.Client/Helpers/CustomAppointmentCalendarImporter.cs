using OutlookInspiredApp.Repository.Service;
using System;
using System.Linq;
using Telerik.Windows.Controls.ScheduleView.ICalendar;

namespace TelerikOutlookInspiredApp
{
    public class CustomAppointmentCalendarImporter : AppointmentCalendarImporter
    {
        protected override Telerik.Windows.Controls.ScheduleView.IAppointment CreateNewAppointment(CalObject vevent)
        {
            return new Appointment();
        }

        protected override Telerik.Windows.Controls.IResource CreateNewResource(CalObject vevent)
        { 
           return new Resource();
        }
    }
}
