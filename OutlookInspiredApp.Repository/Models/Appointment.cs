using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OutlookInspiredApp.Repository.Core;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView.ICalendar;
using Telerik.Windows.Controls.ScheduleView;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class Appointment : IAppointment, ICopyable<IAppointment>, IEditableObject, IExtendedAppointment, IObjectGenerator<IRecurrenceRule>
    {
        public event EventHandler RecurrenceRuleChanged;
        private List<ExceptionOccurrence> exceptionOccurrences;
        private List<ExceptionAppointment> exceptionAppointments;
        private List<Resource> resources;
        private TimeZoneInfo timeZone;
        private IRecurrenceRule recurrenceRule;
        private Appointment editedAppointment;

        [EntityNotSerializableAttribute]
        ITimeMarker IExtendedAppointment.TimeMarker
        {
            get
            {
                return this.TimeMarker as ITimeMarker;
            }
            set
            {
                this.TimeMarker = value as TimeMarker;
            }
        }

        [EntityNotSerializableAttribute]
        ICategory IExtendedAppointment.Category
        {
            get
            {
                return this.Category as ICategory;
            }
            set
            {
                this.Category = value as Category;
            }
        }

        [EntityNotSerializableAttribute]
        Importance IExtendedAppointment.Importance
        {
            get
            {
                return (Importance)this.Importance;
            }
            set
            {
                this.Importance = (int)value;
            }
        }

        [EntityNotSerializableAttribute]
        public TimeZoneInfo TimeZone
        {
            get
            {
                if (this.timeZone == null)
                {
                    return TimeZoneInfo.Local;
                }

                return this.timeZone;
            }

            set
            {
                if (this.timeZone != value)
                {
                    this.timeZone = value;
                    this.OnPropertyChanged("TimeZone");
                }
            }
        }

        [EntityNotSerializableAttribute]
        IList IAppointment.Resources
        {
            get
            {
                return this.Resources;
            }
        }

        public List<Resource> Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = CalendarRepository.GetAppointmentResources(this);
                }

                return this.resources;
            }
            set
            {
                if (this.resources != value)
                {
                    this.resources = value;
                    this.OnPropertyChanged("Resources");
                }
            }
        }

        [EntityNotSerializableAttribute]
        public IRecurrenceRule RecurrenceRule
        {
            get
            {
                if (this.recurrenceRule == null)
                {
                    this.recurrenceRule = this.GetRecurrenceRule(this.RecurrencePattern);
                }

                return this.recurrenceRule;
            }

            set
            {
                if (this.recurrenceRule != value)
                {
                    if (value == null)
                    {
                        this.RecurrencePattern = null;
                    }
                    this.recurrenceRule = value;
                    this.OnPropertyChanged("RecurrenceRule");
                }
            }
        }
         
        void IEditableObject.BeginEdit()
        {
            this.editedAppointment = this.Copy() as Appointment;

            if (this.exceptionOccurrences == null)
            {
                this.exceptionOccurrences = new List<ExceptionOccurrence>();
            }

            if (this.exceptionAppointments == null)
            {
                this.exceptionAppointments = new List<ExceptionAppointment>();
            }

            this.exceptionOccurrences.Clear();
            this.exceptionOccurrences.AddRange(this.ExceptionOccurrences);

            this.exceptionAppointments.Clear();
            this.exceptionAppointments.AddRange(this.ExceptionOccurrences.Select(o => o.ExceptionAppointment).Where(a => a != null));
        }

        void IEditableObject.CancelEdit()
        {
            var exceptionOccurencesToRemove = this.ExceptionOccurrences.Except(this.exceptionOccurrences);
            foreach (var exception in exceptionOccurencesToRemove)
            {
                CalendarRepository.Context.DeleteObject(exception);
                if (exception.Appointment != null)
                {
                    CalendarRepository.Context.DeleteObject(exception.ExceptionAppointment);
                    var exceptionResourcesToRemove = (exception.ExceptionAppointment).ExceptionResources;
                    foreach (var resource in exceptionResourcesToRemove)
                    {
                        CalendarRepository.Context.DeleteObject(resource);
                    }
                }

                if (this.recurrenceRule.Exceptions.Contains(exception))
                {
                    this.recurrenceRule.Exceptions.Remove(exception);
                }
            }

            (this as ICopyable<IAppointment>).CopyFrom(this.editedAppointment);          
        }

        void IEditableObject.EndEdit()
        {
            var temp = this.AppointmentResources;
            var resources = this.Resources;

            foreach (var item in temp)
            {
                CalendarRepository.Context.DeleteObject(item);
            }

            foreach (var resource in resources)
            {
                CalendarRepository.Context.AddToAppointmentResources(new AppointmentResource { Appointments_AppointmentID = this.AppointmentID, Resources_ResourceID = resource.ResourceID });
            }

            if (this.Category != null)
            {
                this.CategoryID = this.Category.CategoryID;
            }

            var removedExceptionAppointments = this.exceptionAppointments.Except(this.ExceptionOccurrences.Select(o => o.Appointment).OfType<ExceptionAppointment>());
            foreach (var exceptionAppointment in removedExceptionAppointments)
            {
                var excResources = exceptionAppointment.ExceptionResources;
                foreach (var item in excResources)
                {
                    CalendarRepository.Context.DeleteObject(item);
                }
            }

            this.SaveAppointment();
        }

        private void SaveAppointment()
        {
            CalendarRepository.Update(this);
            CalendarRepository.SaveChangesAsync();
        }

        public bool Equals(IAppointment other)
        {
            var otherAppointment = other as Appointment;
            return otherAppointment != null &&
                other.Start == this.Start &&
                other.End == this.End &&
                other.Subject == this.Subject &&
                this.CategoryID == otherAppointment.CategoryID &&
                this.TimeMarker == otherAppointment.TimeMarker &&
                this.TimeZone == otherAppointment.TimeZone &&
                this.IsAllDayEvent == other.IsAllDayEvent &&
                this.RecurrenceRule == other.RecurrenceRule;
        }

        public IRecurrenceRule CreateNew()
        {
            return this.CreateDefaultRecurrenceRule();
        }

        public IRecurrenceRule CreateNew(IRecurrenceRule item)
        {
            var recurrenceRule = this.CreateNew();
            recurrenceRule.CopyFrom(item);
            return recurrenceRule;
        }

        public IAppointment Copy()
        {
            IAppointment appointment = new Appointment();
            appointment.CopyFrom(this);
            return appointment;
        }

        public IAppointment ShallowCopy()
        {
            var appointment = new ExceptionAppointment();
            appointment.CopyFrom(this);
            return appointment;
        }

        void ICopyable<IAppointment>.CopyFrom(IAppointment other)
        {
            this.IsAllDayEvent = other.IsAllDayEvent;
            this.Start = other.Start;
            this.End = other.End;
            this.Subject = other.Subject;

            var otherAppointment = other as Appointment;
            if (otherAppointment == null)
                return;

            this.CategoryID = otherAppointment.CategoryID;
            this.TimeMarker = otherAppointment.TimeMarker;
            this.RecurrenceRule = other.RecurrenceRule == null ? null : other.RecurrenceRule.Copy() as RecurrenceRule;
            this.RecurrencePattern = otherAppointment.RecurrencePattern;

            this.Resources.Clear();
            this.Resources.AddRange(otherAppointment.Resources);

            this.Body = otherAppointment.Body;
        }

        private IRecurrenceRule GetRecurrenceRule(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return null;
            }

            var recurrenceRuleGenerator = this as IObjectGenerator<IRecurrenceRule>;
            var recurrenceRule = recurrenceRuleGenerator != null ? recurrenceRuleGenerator.CreateNew() : this.CreateDefaultRecurrenceRule();
            var recurrencePattern = new RecurrencePattern();
            RecurrencePatternHelper.TryParseRecurrencePattern(pattern, out recurrencePattern);
            recurrenceRule.Pattern = recurrencePattern;
            foreach (ExceptionOccurrence exception in this.ExceptionOccurrences)
            {
                recurrenceRule.Exceptions.Add(exception);
            }

            return recurrenceRule;
        }

        private IRecurrenceRule CreateDefaultRecurrenceRule()
        {
            return new RecurrenceRule(this);
        }     
    }
}
