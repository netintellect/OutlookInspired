using System;
using System.Globalization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace TelerikOutlookInspiredApp
{
    public class WorkWeekViewDefinition : WeekViewDefinition
    {
        protected override DateTime GetVisibleRangeStart(DateTime currentDate, CultureInfo culture, DayOfWeek? firstDayOfWeek)
        {
            return CalendarHelper.GetFirstDayOfWeek(currentDate, DayOfWeek.Monday);
        }

        protected override DateTime GetVisibleRangeEnd(DateTime currentDate, CultureInfo culture, DayOfWeek? firstDayOfWeek)
        {
            DateTime endDate = new DateTime();
            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Monday: endDate = currentDate.AddDays(5);
                    break;
                case DayOfWeek.Tuesday: endDate = currentDate.AddDays(4);
                    break;
                case DayOfWeek.Wednesday: endDate = currentDate.AddDays(3);
                    break;
                case DayOfWeek.Thursday: endDate = currentDate.AddDays(2);
                    break;
                case DayOfWeek.Friday: endDate = currentDate.AddDays(1);
                    break;
                default: endDate = currentDate;
                    break;
            }
            return endDate;
        }
    }
}
