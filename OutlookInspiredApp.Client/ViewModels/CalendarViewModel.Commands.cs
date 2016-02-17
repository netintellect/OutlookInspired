using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OutlookInspiredApp.Repository;
using Service = OutlookInspiredApp.Repository.Service;
using ScheduleView = Telerik.Windows.Controls.ScheduleView;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Controls.ScheduleView.ICalendar;
using Telerik.Windows.Data;


namespace TelerikOutlookInspiredApp
{
    public partial class CalendarViewModel
    {
        private string exportValue;

        public DelegateCommand ImportCommand { get; private set; }
        public DelegateCommand ExportCommand { get; private set; }
        public DelegateCommand FindScheduleViewCommand { get; private set; }
        public DelegateCommand OpenDialogCommand { get; private set; }
        public DelegateCommand SelectCalendarCommand { get; private set; }
        public DelegateCommand SetCategoryCommand { get; private set; }
        public DelegateCommand SetTimeMarkerCommand { get; private set; }
        public DelegateCommand SetTodayCommand { get; private set; }
        public DelegateCommand SetWorkWeekCommand { get; private set; }
        public DelegateCommand SetWeekViewCommand { get; private set; }
        public DelegateCommand DiscardCommand { get; set; }
        public DelegateCommand MenuOpenStateChangedCommand { get; private set; }
        public DelegateCommand VisibleRangeChanged { get; private set; }

        public CalendarViewModel()
        {
            this.InitializeHeader();
            this.selectedResourceNames = new List<string>();
            this.ActiveViewDefinitionIndex = 3;

            this.InitializeSelectedDate();
            this.InitializeCommands();
            
            this.TimeMarkers = new RadObservableCollection<Service.TimeMarker>();
            this.Appointments = new RadObservableCollection<Service.Appointment>();

            this.Appointments.CollectionChanged += this.OnAppointmentsCollectionChanged;

            this.LoadData();
            this.UpdateGroupFilter();
        }

        private void InitializeSelectedDate()
        {
            this.SelectedDate = new DateTime(2015, 06, 10);
        }

        private void InitializeHeader()
        {
            this.Header = "Calendar";
        }

        private void InitializeCommands()
        {
            this.ExportCommand = new DelegateCommand(this.OnExportCommandExecuted);
            this.ImportCommand = new DelegateCommand(this.OnImportCommandExecuted);
            this.FindScheduleViewCommand = new DelegateCommand(this.OnFindScheduleViewCommandExecuted);
            this.OpenDialogCommand = new DelegateCommand(this.OnOpenDialogCommandExecuted);
            this.SelectCalendarCommand = new DelegateCommand(this.OnSelectedCalendarCommandExecuted);
            this.SetCategoryCommand = new DelegateCommand(this.OnSetCategoryCommandExecuted, o => this.SelectedAppointment != null);
            this.SetTimeMarkerCommand = new DelegateCommand(this.OnSetTimeMarkerCommandExecuted, o => this.SelectedAppointment != null);
            this.SetTodayCommand = new DelegateCommand(this.OnSetTodayCommandExecuted);
            this.SetWorkWeekCommand = new DelegateCommand(this.OnSetWorkWeekCommandExecuted, o => this.ActiveViewDefinitionIndex != 2);
            this.SetWeekViewCommand = new DelegateCommand(this.OnSetWeekViewCommandExecuted, o => this.ActiveViewDefinitionIndex != 1);
            this.DiscardCommand = new DelegateCommand(this.OnDiscardCommandExecute);
            this.MenuOpenStateChangedCommand = new DelegateCommand(this.OnMenuOpenStateChangedCommandExecuted);
            this.VisibleRangeChanged = new DelegateCommand(this.OnVisibleRangeExecuted, (param) => true);
        }

        public void OnFindScheduleViewCommandExecuted(object obj)
        {
            var scheduleView = obj as RadScheduleView;
            if (scheduleView != null)
            {
                this.ScheduleView = scheduleView;
            }
        }

        private void OnOpenDialogCommandExecuted(object obj)
        {
            RadWindow.Alert(
                new DialogParameters
                {
                    Content = string.Format("{0}'s command executed.", obj.ToString()),
                    Header = "Telerik"
                });
        }

        private void OnSelectedCalendarCommandExecuted(object parameter)
        {
            var args = parameter as SelectionChangedEventArgs;
            this.RemoveUnselectedGroups(args.RemovedItems);
            this.AddSelectedGroups(args.AddedItems);

            this.UpdateSelectedResourceNames();

            this.UpdateGroupFilter();
        }

        private void RemoveUnselectedGroups(IList unselectedGroups)
        {
            foreach (var unselectedGroup in unselectedGroups)
            {
                if (this.SelectedGroups.Contains(unselectedGroup))
                {
                    this.SelectedGroups.Remove(unselectedGroup);
                }
            }
        }

        private void AddSelectedGroups(IList selectedGroups)
        {
            foreach (var selectedGroup in selectedGroups)
            {
                if (!this.SelectedGroups.Contains(selectedGroup))
                {
                    this.SelectedGroups.Add(selectedGroup);
                }
            }
        }

        private void UpdateSelectedResourceNames()
        {
            this.selectedResourceNames.Clear();
            foreach (var item in this.SelectedGroups)
            {
                this.selectedResourceNames.Add(item.ToString());
            }
        }

        public void OnSetCategoryCommandExecuted(object parameter)
        {
            var selectedCategory = parameter as Telerik.Windows.Controls.Category;
            var newCategory = this.GetNewCategory(selectedCategory);
            if (newCategory != null)
            {
                var appointment = this.SelectedAppointment as Service.Appointment;
                ScheduleView.IExceptionOccurrence exceptionToEdit = null;

                if (!(this.SelectedAppointment is Service.Appointment))
                {
                    appointment = (this.SelectedAppointment as ScheduleView.Occurrence).Master as Service.Appointment;
                    if (appointment.RecurrenceRule != null)
                    {
                        exceptionToEdit = appointment.RecurrenceRule.Exceptions.SingleOrDefault(e => (e.Appointment as ScheduleView.IOccurrence) == ((Telerik.Windows.Controls.ScheduleView.Occurrence)(this.SelectedAppointment)).Appointment);
                        if (exceptionToEdit != null)
                        {
                            appointment.RecurrenceRule.Exceptions.Remove(exceptionToEdit);
                            SetCategory((exceptionToEdit.Appointment as Service.Appointment), newCategory);
                            appointment.RecurrenceRule.Exceptions.Add(exceptionToEdit);
                        }
                    }
                }

                var appointmentToEdit = this.Appointments.FirstOrDefault(a => a.AppointmentID == appointment.AppointmentID);
                if (appointmentToEdit != null)
                {
                    SetCategory(appointmentToEdit, newCategory);
                }

                this.ReplaceItem(appointmentToEdit);

                CalendarRepository.SaveChangesAsync();
            }
        }

        public Service.Category GetNewCategory(Telerik.Windows.Controls.Category selectedCategory)
        {
            var newCategory = this.Categories.FirstOrDefault(c => c.DisplayName == selectedCategory.DisplayName);
            if (newCategory != null)
            {
                return newCategory;
            }
            return null;
        }
        private static void SetCategory(Service.Appointment appointment, Service.Category category)
        {
            appointment.Category = category;
            appointment.CategoryID = category.CategoryID;
        }

        private void ReplaceItem(Service.Appointment appointmentToEdit)
        {
            if (appointmentToEdit != null)
            {
                this.shouldProcessCollectionChanged = false;

                var index = this.Appointments.IndexOf(appointmentToEdit);
                this.Appointments.Remove(appointmentToEdit);
                this.Appointments.Insert(index, appointmentToEdit);

                this.shouldProcessCollectionChanged = true;
            }
        }

        public void OnSetTimeMarkerCommandExecuted(object parameter)
        {
            var selectedTimeMarker = parameter as Telerik.Windows.Controls.TimeMarker;
            var newTimeMarker = selectedTimeMarker != null ? this.GetNewTimeMarker(selectedTimeMarker) : null;

            if (newTimeMarker != null)
            {
                var appointment = this.SelectedAppointment as Service.Appointment;

                ScheduleView.IExceptionOccurrence exceptionToEdit = null;
                if (appointment == null)
                {
                    this.SetTimeMarkerToOccurrence(ref appointment, newTimeMarker, ref exceptionToEdit);
                }

                this.SetTimeMarkerToAppointment(appointment, newTimeMarker);


                CalendarRepository.Update(appointment);
                CalendarRepository.SaveChangesAsync();
            }
        }

        private Service.TimeMarker GetNewTimeMarker(TimeMarker selectedTimeMarker)
        {
            var newTimeMarker = this.TimeMarkers.FirstOrDefault(t => t.TimeMarkerName == selectedTimeMarker.TimeMarkerName);
            if (newTimeMarker != null)
            {
                return newTimeMarker;
            }
            return null;
        }

        private void SetTimeMarkerToAppointment(Service.Appointment appointment, Service.TimeMarker newTimeMarker)
        {
            var appointmentToEdit = (from app in this.Appointments where app.Equals(appointment) select app).FirstOrDefault();
            if (appointmentToEdit != null)
            {
                SetTimeMarker(appointmentToEdit, newTimeMarker);
                this.ReplaceItem(appointmentToEdit);
            }
        }

        private void SetTimeMarkerToOccurrence(ref Service.Appointment appointment, Service.TimeMarker newTimeMarker, ref ScheduleView.IExceptionOccurrence exceptionToEdit)
        {
            appointment = (this.SelectedAppointment as ScheduleView.Occurrence).Master as Service.Appointment;
            if (appointment != null && appointment.RecurrenceRule != null)
            {
                exceptionToEdit = appointment.RecurrenceRule.Exceptions.SingleOrDefault(e => (e.Appointment as ScheduleView.IOccurrence) == ((ScheduleView.Occurrence)(this.SelectedAppointment)).Appointment);
                if (exceptionToEdit != null)
                {
                    appointment.RecurrenceRule.Exceptions.Remove(exceptionToEdit);
                    SetTimeMarker(exceptionToEdit.Appointment as Service.Appointment, newTimeMarker);

                    appointment.RecurrenceRule.Exceptions.Add(exceptionToEdit);
                }

                SetTimeMarker(appointment, newTimeMarker);

                this.ReplaceItem(appointment);
            }
        }

        private static void SetTimeMarker(Service.Appointment appointment, Service.TimeMarker newTimeMarker)
        {
            appointment.TimeMarker = newTimeMarker;
            appointment.TimeMarkerID = newTimeMarker.TimeMarkerID;
        }

        public void OnSetTodayCommandExecuted(object parameter)
        {
            this.SelectedDate = DateTime.Today;
        }

        public void OnSetWorkWeekCommandExecuted(object parameter)
        {
            this.ActiveViewDefinitionIndex = 2;
        }

        public void OnSetWeekViewCommandExecuted(object parameter)
        {
            this.ActiveViewDefinitionIndex = 1;
        }

        private void OnMenuOpenStateChangedCommandExecuted(object parameter)
        {
            var ribbon = parameter as RadRichTextBoxRibbonUI;
            if (ribbon.IsApplicationMenuOpen)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.DiscardCommand.Execute(null);
                }));
            }
        }

        /* Import and Export commands */
        public string ExportValue
        {
            get
            {
                return this.exportValue;
            }
            set
            {
                if (this.exportValue != value)
                {
                    this.exportValue = value;
                    this.OnPropertyChanged(() => this.ExportValue);
                }
            }
        }
        private void OnExportCommandExecuted(object parameter)
        {
            this.ExportToFile();
        }

        private void ExportToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".ics";
            dialog.Filter = "ICalendar file (.ics)|*.ics";

            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        AppointmentCalendarExporter exporter = new AppointmentCalendarExporter();
                        exporter.Export(this.Appointments.OfType<ScheduleView.IAppointment>(), writer);
                    }
                }
            }
        }

        private void OnImportCommandExecuted(object obj)
        {
            this.ImportFromFile();
        }

        private void ImportFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".ics";
            dialog.Filter = "ICalendar file (.ics)|*.ics";
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                using (TextReader txtReader = new StreamReader(dialog.FileName))
                {
                    this.Import(txtReader);
                }
            }
        }

        private void Import(TextReader reader)
        {
            try
            {
                var importer = new CustomAppointmentCalendarImporter();
                IEnumerable<ScheduleView.IAppointment> importedAppointments = importer.Import(reader);

                this.shouldProcessCollectionChanged = false;
                foreach (Service.Appointment app in importedAppointments)
                {
                    this.Appointments.Add(app);
                    CalendarRepository.InsertAppointment(app);
                }

                CalendarRepository.SaveChangesAsync();
                this.shouldProcessCollectionChanged = true;
            }
            catch (CalendarParseException ex)
            {
                MessageBox.Show(ex.Message, "CalendarParseException", MessageBoxButton.OK);
            }
        }

        private void OnDiscardCommandExecute(object obj)
        {
            // Execute any actions that should be triggered when the RadRichTextBoxRibbonUI's ApplicationMenuOpen is opened.
        }

        private void OnVisibleRangeExecuted(object parameter)
        {
            var range = parameter as ScheduleView.DateSpan;
            if (this.currentRange == null || !this.currentRange.Equals(range))
            {
                this.LoadAppointments(range);
                this.currentRange = range;
            }
        }

        private void InvalidateGotoViewDefinitionCommands()
        {
            if (this.SetWeekViewCommand != null && this.SetWorkWeekCommand != null)
            {
                this.SetWeekViewCommand.InvalidateCanExecute();
                this.SetWorkWeekCommand.InvalidateCanExecute();
            }
        }
    }
}
