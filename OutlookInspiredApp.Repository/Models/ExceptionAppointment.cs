using OutlookInspiredApp.Repository.Core;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class ExceptionAppointment : IEditableObject, IAppointment, IExtendedAppointment, IObjectGenerator<IRecurrenceRule>
    {
        public event EventHandler RecurrenceRuleChanged;

        private TimeZoneInfo timeZone;
        private IRecurrenceRule recurrenceRule;
        private IList resources;

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
        public IRecurrenceRule RecurrenceRule
        {
            get
            {
                return this.recurrenceRule;
            }

            set
            {
                if (this.recurrenceRule != value)
                {
                    this.recurrenceRule = value;
                    this.OnPropertyChanged("RecurrenceRule");
                }
            }
        }

        [EntityNotSerializableAttribute]
        public IList Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = new ObservableCollection<Resource>();
                    var resources = ConnectionManager.Context.ExceptionResources.Where(x => x.ExceptionAppointmentID == this.ExceptionID).Select(x => x.Resource);
                    this.resources.AddRange(resources);

                    ((INotifyCollectionChanged)this.resources).CollectionChanged += this.OnResourcesCollectionChanged;
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

        public IAppointment Copy()
        {
            IAppointment appointment = new ExceptionAppointment();
            appointment.CopyFrom(this);
            return appointment;
        }

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
        }

        public bool Equals(IAppointment other)
        {
            var otherAppointment = other as ExceptionAppointment;
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
            throw new InvalidOperationException();
        }

        public IRecurrenceRule CreateNew(IRecurrenceRule item)
        {
            throw new InvalidOperationException();
        }

        public void CopyFrom(IAppointment other)
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

            this.Resources.Clear();
            this.Resources.AddRange(otherAppointment.Resources);

            this.Body = otherAppointment.Body;
        }

        private void OnResourcesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var resource in e.NewItems.OfType<Resource>())
                    {
                        this.ExceptionResources.Add(
                        new ExceptionResource
                        {
                            ExceptionAppointmentID = this.ExceptionID,
                            ExceptionAppointmentResourcesID = resource.ResourceID
                        });
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var resource in e.OldItems.OfType<Resource>())
                    {
                        var contextExceptionResourcesToRemove = ConnectionManager.Context.ExceptionResources.Where(x => x.ExceptionAppointmentResourcesID == resource.ResourceID && x.ExceptionAppointmentID == this.ExceptionID);
                        if (contextExceptionResourcesToRemove.Any())
                        {
                            foreach (var item in contextExceptionResourcesToRemove)
                            {
                                CalendarRepository.Context.DeleteObject(item);
                            }
                        }
                        else
                        {
                            var resourceExceptionResourcesToRemove = resource.ExceptionResources.Where(x => x.ExceptionAppointmentResourcesID == resource.ResourceID && x.ExceptionAppointmentID == this.ExceptionID);
                            foreach (var item in resourceExceptionResourcesToRemove)
                            {
                                CalendarRepository.Context.Detach(item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
