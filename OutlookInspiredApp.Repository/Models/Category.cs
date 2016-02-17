using System;
using System.Linq;
using Telerik.Windows.Controls;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class Category : ICategory
    {
        public bool Equals(ICategory other)
        {
            return this.DisplayName == other.DisplayName &&
                this.CategoryName == other.CategoryName;
        }
    }
}
