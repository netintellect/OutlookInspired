using OutlookInspiredApp.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutlookInspiredApp.Repository
{
    public class CalendarRepository : RepositoryBase
    {
        private static List<Appointment> appointmentsCache;

        public static List<Appointment> AppointmentsCache
        {
            get
            {
                if (appointmentsCache == null)
                {
                    appointmentsCache = GetAppoinements();
                }
                return appointmentsCache;
            }
            private set
            {
                if (appointmentsCache != value)
                {
                    appointmentsCache = value;
                }
            }
        }

        private static IEnumerable<AppointmentResource> appointmentResourcesCache;

        public static IEnumerable<AppointmentResource> AppointmentResourcesCache
        {
            get
            {
                if (appointmentResourcesCache == null)
                {
                    appointmentResourcesCache = GetAppointmentResources();
                }
                return appointmentResourcesCache;
            }
            private set
            {
                if (appointmentResourcesCache != value)
                {
                    appointmentResourcesCache = value;
                }
            }
        }


        private static List<Appointment> GetAppoinements()
        {
            var appointments = Context.Appointments
                      .Expand(a => a.AppointmentResources)
                      .Expand(a => a.ExceptionOccurrences)
                      .Expand(a => a.Category)
                      .Expand(a => a.TimeMarker);

            return appointments.ToList();
        }

        private static IEnumerable<AppointmentResource> GetAppointmentResources()
        {
            var appointmentResources = Context.AppointmentResources
                    .Expand(a => a.Appointment)
                    .Expand(a => a.Resource);

            return appointmentResources.ToList();
        }

        public static void UpdateAppointmentsCache()
        {
            appointmentsCache = GetAppoinements();
        }

        public static void GetAppointmentsByRange(DateTime start, DateTime end, Action<IEnumerable<Appointment>> calback)
        {
            ExecuteQueryAsync<Appointment>(GetAppointmentsByRange(start, end), calback);
        }

        private static async Task<IEnumerable<Appointment>> GetAppointmentsByRange(DateTime start, DateTime end)
        {
            if (AppointmentsCache != null)
            {
                var result = AppointmentsCache.Where(a => ((a.Start >= start && a.End <= end))).ToList<Appointment>();
                var appointmentsWithRecurrencePattern = GetAppointmentsWithRecurrencePattern();

                foreach (var item in appointmentsWithRecurrencePattern)
                {
                    if (RecurrenceHelper.IsOccurrenceInRange(item.RecurrencePattern, start, end) && !result.Contains(item))
                    {
                        result.Add(item);
                    }
                }

                var appointmentsWithExceptionOccurrences = GetAppointmentsWithExceptionOccurrences(end);
                foreach (var item in appointmentsWithExceptionOccurrences)
                {
                    var hasExceptionOccurenceWithinDateRange = HasExceptionOccurenceWithinDateRange(item, start, end);
                    bool isContainedInResult = IsContainedInResult(result, item);
                    if (hasExceptionOccurenceWithinDateRange && isContainedInResult)
                    {
                        result.Add(item);
                    }
                }
                return result.AsQueryable<Appointment>();
            }

            return Enumerable.Empty<Appointment>();
        }

        private static IEnumerable<Appointment> GetAppointmentsWithRecurrencePattern()
        {
            return AppointmentsCache.Where(a => a.RecurrencePattern != string.Empty || a.RecurrencePattern != null);
        }

        private static IEnumerable<Appointment> GetAppointmentsWithExceptionOccurrences(DateTime end)
        {
            return AppointmentsCache.Where(a => a.Start < end && a.ExceptionOccurrences.Count != 0); ;
        }

        private static bool HasExceptionOccurenceWithinDateRange(Appointment appointment, DateTime start, DateTime end)
        {
            var result = appointment.ExceptionOccurrences.Any(e => e.ExceptionAppointment != null && e.ExceptionAppointment.Start >= start && e.ExceptionAppointment.End <= end);

            return result;
        }
        private static bool IsContainedInResult(List<Appointment> appointments, Appointment appointment)
        {
            var result = !appointments.Contains(appointment);

            return result;
        }

        public static List<Resource> GetAppointmentResources(Telerik.Windows.Controls.ScheduleView.IAppointment currentAppointment)
        {
            var appointment = currentAppointment as Appointment;
            var resources = AppointmentResourcesCache.Where(r => r.Appointments_AppointmentID == appointment.AppointmentID).Select(r => r.Resource).ToList();

            return resources;
        }

        public static void GetResources(Action<IEnumerable<Resource>> callback)
        {
            ExecuteQueryAsync<Resource>(GetResourcesAsync(), callback);
        }

        private static async Task<IEnumerable<Resource>> GetResourcesAsync()
        {
            var resources = Context.Resources.Expand(r => r.ResourceType);
            if (resources == null)
            {
                return Enumerable.Empty<Resource>().AsQueryable();
            }

            return resources;
        }

        public static void GetResourceTypes(Action<IEnumerable<ResourceType>> callback)
        {
            ExecuteQueryAsync<ResourceType>(GetResourceTypesAsync(), callback);
        }

        private static async Task<IEnumerable<ResourceType>> GetResourceTypesAsync()
        {
            var resourceTypes = await Task.Run(() => Context.ResourceTypes.Expand(r => r.Resources));
            if (resourceTypes == null)
            {
                return Enumerable.Empty<ResourceType>().AsQueryable();
            }

            return resourceTypes;
        }

        public static void GetTimeMarkers(Action<IEnumerable<TimeMarker>> callback)
        {
            ExecuteQueryAsync<TimeMarker>(GetTimeMarkersAsync(), callback);
        }

        private static async Task<IEnumerable<TimeMarker>> GetTimeMarkersAsync()
        {
            var timeMarkers = Context.TimeMarkers;
            if (timeMarkers == null)
            {
                return Enumerable.Empty<TimeMarker>().AsQueryable();
            }

            return timeMarkers;
        }

        public async static void LoadData()
        {
            AppointmentsCache = (await GetAppoinementsAsync()).ToList();
            AppointmentResourcesCache = (await GetAppointmentResourcesAsync()).ToList();
        }

        private async static Task<IEnumerable<Appointment>> GetAppoinementsAsync()
        {
            return await Task.Run(() => GetAppoinements());
        }

        private async static Task<IEnumerable<AppointmentResource>> GetAppointmentResourcesAsync()
        {
            return await Task.Run(() => GetAppointmentResources());
        }

        public static void InsertAppointment(Appointment appointment)
        {
            Context.AddToAppointments(appointment);
            AppointmentsCache.Add(appointment);
        }

        public override bool Contains(object entity)
        {
            return Context.Appointments.Where(a => a.AppointmentID == ((Appointment)entity).AppointmentID).Count() >= 1;
        }
    }
}
