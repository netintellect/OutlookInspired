using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelerikOutlookInspiredApp
{
    public class OutlookRepository : IDisposable
    {
        private static OutlookEntities context;
        public static OutlookEntities Context
        {
            get
            {
                if (context == null)
                {
                    context = new OutlookEntities();
                }
                return context;
            }
        }

        internal static List<Folder> GetParentFolders()
        {
            if (Context != null)
            {
                return new List<Folder>(Context.Folders.Where(f => f.ParentFolderID == null));
            }

            return new List<Folder>();
        }

        public static void SaveChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);                       
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }  
        }

        public static bool SaveChanges(Action action)
        {
            var isSubmited = Context.SaveChanges() >= 0;
            if (action != null && isSubmited)
            {
                action();
            }
            return isSubmited;
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                context = null;
            }

            GC.SuppressFinalize(this);
        }

        public static int GetDeletedItemsFolderID()
        {
            return Context.Folders.FirstOrDefault(f => f.Name == "Deleted Items").FolderID;
        }

        public static int GetSentItemsFolderID()
        {
            return Context.Folders.FirstOrDefault(f => f.Name == "Sent Items").FolderID;
        }

        public static IQueryable<Appointment> GetAppointmentsByRange(DateTime start, DateTime end)
        {
            var ids = GetAppointmentsIDsByRange(start, end);

            var result = Context.Appointments.Where(a => ids.Contains(a.AppointmentID)).ToList<Appointment>();

            foreach (var item in Context.Appointments.Where(a => !string.IsNullOrEmpty(a.RecurrencePattern)))
            {
                if (RecurrenceHelper.IsOccurrenceInRange(item.RecurrencePattern, start, end) && !result.Contains(item))
                {
                    result.Add(item);
                }
            }

            foreach (var item in Context.Appointments.Where(a => a.Start < end && a.ExceptionOccurrences.Count != 0))
            {
                if (item.ExceptionOccurrences.Any(e => e.ExceptionAppointment != null && e.ExceptionAppointment.Start >= start && e.ExceptionAppointment.End <= end) && !result.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result.AsQueryable<Appointment>();
        }

        private static int[] GetAppointmentsIDsByRange(DateTime start, DateTime end)
        {
            var result = Context.Appointments.Where(a => ((a.Start >= start && a.End <= end) || (a.Start <= start && a.End <= end) || (a.Start >= start && a.End >= end) || (a.Start <= start && a.End >= end)));

            return result.OfType<Appointment>().Select(e => e.AppointmentID).ToArray();
        }
    }
}
