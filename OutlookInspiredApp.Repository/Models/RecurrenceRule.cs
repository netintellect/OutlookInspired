using OutlookInspiredApp.Repository.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.Controls.ScheduleView.ICalendar;

namespace OutlookInspiredApp.Repository.Service
{
    public class RecurrenceRule : ViewModelBase, IRecurrenceRule
    {
        private RecurrencePattern pattern;

        public RecurrenceRule()
        {
            this.Exceptions = new ObservableCollection<IExceptionOccurrence>();
        }

        public RecurrenceRule(Appointment appointment)
            : this()
        {
            this.MasterAppointment = appointment;
        }

        [EntityNotSerializableAttribute]
        public Appointment MasterAppointment { get; private set; }

        [EntityNotSerializableAttribute]
        public RecurrencePattern Pattern
        {
            get
            {
                return pattern;
            }
            set
            {
                if (this.pattern != value)
                {
                    this.pattern = value;
                    this.MasterAppointment.RecurrencePattern = RecurrencePatternHelper.RecurrencePatternToString(pattern);
                    this.OnPropertyChanged("Pattern");
                }
            }
        }

        [EntityNotSerializableAttribute]
        public ICollection<IExceptionOccurrence> Exceptions
        {
            get;
            private set;
        }

        public IRecurrenceRule Copy()
        {
            var rule = new RecurrenceRule();
            rule.CopyFrom(this);
            return rule;
        }

        public IExceptionOccurrence CreateNew()
        {
            var exceptionOccurence = new ExceptionOccurrence();
            exceptionOccurence.Appointment = this.MasterAppointment;
            CalendarRepository.Context.AddToExceptionOccurrences(exceptionOccurence);

            return exceptionOccurence;
        }

        public IExceptionOccurrence CreateNew(IExceptionOccurrence item)
        {
            var exceptionOccurrence = this.CreateNew();
            exceptionOccurrence.CopyFrom(item);
            return exceptionOccurrence;
        }

        public void CopyFrom(IRecurrenceRule other)
        {
            if (this.GetType().FullName != other.GetType().FullName)
            {
                throw new ArgumentException("Invalid type");
            }

            if (other is RecurrenceRule)
            {
                this.MasterAppointment = (other as RecurrenceRule).MasterAppointment;
            }

            this.Pattern = other.Pattern.Copy();
            this.Exceptions.Clear();

            var exceptions = other.Exceptions;
            other.Exceptions.Clear();

            this.Exceptions = exceptions;
        }

        public IAppointment CreateExceptionAppointment(IAppointment master)
        {
            return (master as Appointment).ShallowCopy();
        }
    }
}
