using OutlookInspiredApp.Repository.Core;
using System;
using System.Linq;
using Telerik.Windows.Controls;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class Resource : IResource
    {
        [EntityNotSerializableAttribute]
        string IResource.ResourceType
        {
            get
            {
                return this.ResourceType != null ? this.ResourceType.Name : string.Empty;
            }
            set
            {
                if (this.ResourceType != null && this.ResourceType.Name != value)
                {
                    this.ResourceType.Name = value;
                    this.OnPropertyChanged("ResourceType");
                }
            }
        }

        public override string ToString()
        {
            return this.DisplayName;
        }

        public bool Equals(IResource other)
        {
            return other != null && other.ResourceName == this.ResourceName && this.ResourceType != null && other.ResourceType == this.ResourceType.Name;
        }
    }
}
