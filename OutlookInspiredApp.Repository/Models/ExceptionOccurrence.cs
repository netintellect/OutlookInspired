using OutlookInspiredApp.Repository.Core;
using System;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Controls.ScheduleView;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class ExceptionOccurrence : IExceptionOccurrence
    {
        [EntityNotSerializableAttribute]
        IAppointment IExceptionOccurrence.Appointment
        {
            get
            {
                return this.ExceptionAppointment;
            }
            set
            {
                 if (this.ExceptionAppointment != value)
                 {
                     if (value == null)
                     {
                         CalendarRepository.Context.DeleteObject(this.ExceptionAppointment);
                     }

                     this.ExceptionAppointment = value as ExceptionAppointment;
                     this.OnPropertyChanged("Appointment");
                 }
            }
        }

        public IExceptionOccurrence Copy()
        {
            var exception = new ExceptionOccurrence();
            exception.CopyFrom(this);
         
            return exception;
        }

        public void CopyFrom(IExceptionOccurrence other)
        {
            if (this.GetType().FullName != other.GetType().FullName)
            {
                throw new ArgumentException("Invalid type");
            }

            this.ExceptionDate = other.ExceptionDate;
            if (other.Appointment != null)
            {
                this.Appointment = other.Appointment.Copy() as Appointment;
            }
        }
    }
}
