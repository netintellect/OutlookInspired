using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace TelerikOutlookInspiredApp
{
    public class OutlookSection : INotifyPropertyChanged
    {
        private ViewModelBase viewModel;
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand Command { get; set; }

        public IEnumerable<object> Content { get; set; }

        public string Name { get; set; }

        public string IconPath { get; set; }

        public string MinimizedIconPath { get; set; }
     
        public ViewModelBase ViewModel
        {
            get
            {
                return this.viewModel;
            }
            set
            {
                if (this.viewModel != value)
                {
                    this.viewModel = value;
                    this.OnPropertyChanged("ViewModel");
                }
            }
        }  

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}