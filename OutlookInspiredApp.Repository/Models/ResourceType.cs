using OutlookInspiredApp.Repository.Core;
using System;
using System.Linq;
using Telerik.Windows.Controls;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class ResourceType : IResourceType
    {
        [EntityNotSerializableAttribute]
        System.Collections.IList IResourceType.Resources
        {
            get
            { 
                return this.Resources.ToList();
            }
        }
    }
}
