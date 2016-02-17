using System;
using System.Linq;
using Telerik.Windows.Controls;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class TimeMarker : ITimeMarker
    {
        public bool Equals(ITimeMarker other)
        {
            return this.TimeMarkerName != other.TimeMarkerName;
        }
    }
}
