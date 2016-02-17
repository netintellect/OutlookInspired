using OutlookInspiredApp.Repository;
using OutlookInspiredApp.Repository.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Controls = Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public partial class MainViewModel : Controls.ViewModelBase
    {
        private ObservableCollection<OutlookSection> outlookSections;
        private OutlookSection selectedOutlookSection;
        private Controls.ViewModelBase selectedViewModel;

        public MainViewModel()
        {
            this.InitializeCommands();
        }

        /// <summary>
        /// Gets or sets OutlookSections and notifies for changes
        /// </summary>
        public ObservableCollection<OutlookSection> OutlookSections
        {
            get
            {
                if (this.outlookSections == null)
                {
                    this.outlookSections = this.GetOutlookSections();
                }
                return this.outlookSections;
            }
        }

        /// <summary>
        /// Gets or sets SelectedOutlookSection and notifies for changes. It could be either the Mail or Calendar view.
        /// </summary>
        public OutlookSection SelectedOutlookSection
        {
            get
            {
                return this.selectedOutlookSection;
            }

            set
            {
                if (this.selectedOutlookSection != value)
                {
                    this.selectedOutlookSection = value;
                    this.OnPropertyChanged(() => this.SelectedOutlookSection);
                }
            }
        }
        public Controls.ViewModelBase SelectedViewModel
        {
            get
            {
                if (this.selectedViewModel == null)
                {
                    this.selectedViewModel = this.SetDefaultSelectedViewModel();
                }
                return this.selectedViewModel;
            }
            set
            {
                if (this.selectedViewModel != value)
                {
                    this.selectedViewModel = value;
                    this.OnPropertyChanged("SelectedViewModel");
                }
            }
        }

        private Controls.ViewModelBase SetDefaultSelectedViewModel()
        {
            if (this.OutlookSections != null)
            {
                var firstSection = this.OutlookSections.FirstOrDefault();
                if (firstSection != null)
                {
                    return firstSection.ViewModel;
                }
            }
            return null;
        }

        public ObservableCollection<OutlookSection> GetOutlookSections()
        {
            var result = new ObservableCollection<OutlookSection>();

            var mailViewModel = new MailViewModel();
            result.Add(new OutlookSection
              {
                  Name = "Mail",
                  Content = MailRepository.ParentFolders,
                  IconPath = "../Images/mail_32x32.png",
                  MinimizedIconPath = "../Images/email_16x16.png",
                  ViewModel = mailViewModel
              });

            var viewModel = new CalendarViewModel();
            result.Add(new OutlookSection
            {
                Name = "Calendar",
                Content = viewModel.Resources,
                Command = viewModel.SelectCalendarCommand,
                IconPath = "../Images/calendar_32x32.png",
                MinimizedIconPath = "../Images/calendar_16x16.png",
                ViewModel = viewModel
            });

            return result;
        }
    }

    internal static class ApplicationThemeViewModel
    {
        public static string SelectedTheme
        {
            get;
            set;
        }
    }
}