using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using OutlookInspiredApp.Repository;
using OutlookInspiredApp.Repository.Service;
using Controls = Telerik.Windows.Controls;
using ScheduleView = Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.Data;
using System.Threading.Tasks;

namespace TelerikOutlookInspiredApp
{
    public partial class CalendarViewModel : Controls.ViewModelBase
    {
        private ScheduleView.IOccurrence selectedAppointment;
        private RadObservableCollection<Appointment> appointments;
        private RadObservableCollection<TimeMarker> timeMarkers;
        private IEnumerable<Category> categories;
        private IEnumerable<Resource> resources;
        private RadObservableCollection<ResourceType> resourceTypes;
        private Func<object, bool> groupFilter;
        private Controls.GroupDescriptionCollection groupDescriptions;
        private readonly List<string> selectedResourceNames;
        private List<object> selectedGroups;
        private int activeViewDefinitionIndex;
        private bool shouldProcessCollectionChanged;
        private DateTime selectedDate;
        private ScheduleView.DateSpan currentRange;
        private string header;
        private Controls.RadScheduleView scheduleView;    

        /// <summary>
        /// Gets or sets the header displayed in the OutlookBar section.
        /// </summary>
        public string Header
        {
            get
            {
                return this.header;
            }
            set
            {
                if (this.header != value)
                {
                    this.header = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected date from the calendar.
        /// </summary>
        public DateTime SelectedDate
        {
            get
            {
                return this.selectedDate;
            }
            set
            {
                if (this.selectedDate != value)
                {
                    this.selectedDate = value;
                    this.OnPropertyChanged("SelectedDate");
                }
            }
        }

        /// <summary>
        /// Gets GroupDescriptions
        /// </summary>
        public Controls.GroupDescriptionCollection GroupDescriptions
        {
            get
            {
                if (this.groupDescriptions == null)
                {
                    var resourceGroupDescription = new Controls.ResourceGroupDescription();
                    resourceGroupDescription.ResourceType = "CalendarType";
                    this.groupDescriptions = new Controls.GroupDescriptionCollection() { resourceGroupDescription, new Controls.DateGroupDescription() };
                }

                return this.groupDescriptions;
            }
        }

        /// <summary>
        /// Gets or sets GroupFilter and notifies for changes
        /// </summary>
        public Func<object, bool> GroupFilter
        {
            get
            {
                return this.groupFilter;
            }
            set
            {

                this.groupFilter = value;
                this.OnPropertyChanged(() => this.GroupFilter);
            }
        }

        /// <summary>
        /// Gets or sets the instance of the RadScheduleView.
        /// </summary>
        public Controls.RadScheduleView ScheduleView
        {
            get
            {
                return this.scheduleView;
            }
            set
            {
                if (this.scheduleView != value)
                {
                    this.scheduleView = value;
                    this.OnPropertyChanged("ScheduleView");
                }
            }
        }

        public IEnumerable<Resource> Resources
        {
            get
            {
                if (this.resources == null)
                {
                    CalendarRepository.GetResources(items =>
                    {
                        this.resources = new ObservableCollection<Resource>(items);
                        this.OnPropertyChanged("Resources");
                    });
                }
                return this.resources;
            }
        }

        /// <summary>
        /// Gets or sets ResourceTypes and notifies for changes
        /// </summary>
        public RadObservableCollection<ResourceType> ResourceTypes
        {
            get
            {
                if (this.resourceTypes == null)
                {
                    CalendarRepository.GetResourceTypes(items =>
                    {
                        this.resourceTypes = new RadObservableCollection<ResourceType>(items);
                        this.OnPropertyChanged("ResourceTypes");
                    });
                }
                return this.resourceTypes;
            }
        }

        /// <summary>
        /// Gets or sets TimeMarkers and notifies for changes
        /// </summary>
        public RadObservableCollection<TimeMarker> TimeMarkers
        {
            get
            {
                return this.timeMarkers;
            }
            set
            {
                if (this.timeMarkers != value)
                {
                    this.timeMarkers = value;
                    this.OnPropertyChanged(() => this.TimeMarkers);
                }
            }
        }

        /// <summary>
        /// Gets or sets Categories and notifies for changes
        /// </summary>
        public IEnumerable<Category> Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = RepositoryBase.Categories;
                }
                return this.categories;
            }

            set
            {
                if (this.categories != value)
                {
                    this.categories = value;
                    this.OnPropertyChanged(() => this.Categories);
                }
            }
        }

        /// <summary>
        /// Gets or sets Appointments and notifies for changes
        /// </summary>
        public RadObservableCollection<Appointment> Appointments
        {
            get
            {
                return this.appointments;
            }

            set
            {
                if (this.appointments != value)
                {
                    this.appointments = value;
                    this.OnPropertyChanged(() => this.Appointments);
                }
            }
        }

        /// <summary>
        /// Gets or sets SelectedAppointment and notifies for changes
        /// </summary>
        public ScheduleView.IOccurrence SelectedAppointment
        {
            get
            {
                return this.selectedAppointment;
            }

            set
            {
                if (this.selectedAppointment != value)
                {
                    this.selectedAppointment = value;
                    this.SetCategoryCommand.InvalidateCanExecute();
                    this.SetTimeMarkerCommand.InvalidateCanExecute();
                    this.OnPropertyChanged(() => this.SelectedAppointment);
                }
            }
        }

        /// <summary>
        /// Gets or sets ActiveViewDefinitionIndex and notifies for changes
        /// </summary>
        public int ActiveViewDefinitionIndex
        {
            get
            {
                return this.activeViewDefinitionIndex;
            }

            set
            {
                if (this.activeViewDefinitionIndex != value)
                {
                    this.activeViewDefinitionIndex = value;
                    this.InvalidateGotoViewDefinitionCommands();
                    this.OnPropertyChanged(() => this.ActiveViewDefinitionIndex);
                }
            }
        }

        /// <summary>
        /// Gets calendar groups that have been selected.
        /// </summary>
        public List<object> SelectedGroups
        {
            get
            {
                if (this.selectedGroups == null)
                {
                    this.selectedGroups = new List<object>();
                }
                return this.selectedGroups;
            }
        }

        private void OnAppointmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.shouldProcessCollectionChanged)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var newAppointment = e.NewItems == null ? null : e.NewItems[0] as Appointment;
                    if (newAppointment != null)
                    {
                        InitializeDate(newAppointment);

                        CalendarRepository.InsertAppointment(newAppointment);
                        CalendarRepository.UpdateAppointmentsCache();
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var appointment = e.OldItems == null ? null : e.OldItems[0] as Appointment;
                    if (appointment != null)
                    {
                        if (appointment.RecurrenceRule != null)
                        {
                            this.RemoveOccurrences(appointment);
                        }

                        this.RemoveResources(appointment);

                        CalendarRepository.UpdateAppointmentsCache();
                        CalendarRepository.Context.DeleteObject(appointment);
                        CalendarRepository.SaveChangesAsync();
                    }
                }
            }
        }

        private void InitializeDate(Appointment newAppointment)
        {
            newAppointment.Start = newAppointment.End = DateTime.Now;
        }

        private void RemoveOccurrences(Appointment appointment)
        {
            var tempList = appointment.RecurrenceRule.Exceptions;

            foreach (ExceptionOccurrence item in tempList)
            {
                CalendarRepository.Context.DeleteObject(item);
            }
        }

        private void RemoveResources(Appointment appointment)
        {
            var tempAppList = CalendarRepository.Context.AppointmentResources.Where(i => i.Appointments_AppointmentID == appointment.AppointmentID);

            foreach (var item in tempAppList)
            {
                CalendarRepository.Context.DeleteObject(item);
            }
        }

        private void LoadAppointments(ScheduleView.DateSpan dateSpan)
        {
            this.shouldProcessCollectionChanged = false;

            this.Appointments.Clear();
            CalendarRepository.GetAppointmentsByRange(dateSpan.Start, dateSpan.End, items =>
                {
                    this.Appointments.AddRange(items);
                });
            this.shouldProcessCollectionChanged = true;
        }

        private void LoadData()
        {
            Task.Run(() => CalendarRepository.LoadData());
            CalendarRepository.GetResourceTypes(items =>
            {
                this.resourceTypes = new RadObservableCollection<ResourceType>(items);
                this.OnPropertyChanged("ResourceTypes");
            });

            CalendarRepository.GetTimeMarkers(items =>
            {
                this.timeMarkers = new RadObservableCollection<TimeMarker>(items);
                this.OnPropertyChanged("TimeMarkers");
            });
        }

        private void UpdateGroupFilter()
        {
            this.GroupFilter = new Func<object, bool>(this.GroupFilterFunc);
        }

        private bool GroupFilterFunc(object groupName)
        {
            var resource = groupName as Controls.IResource;

            return resource == null ? true : this.selectedResourceNames.Contains(resource.ResourceName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
