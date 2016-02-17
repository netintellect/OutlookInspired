using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OutlookInspiredApp.Repository.Service;
using Telerik.Windows.Data;

namespace TelerikOutlookInspiredApp
{
    public class HierarchyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var folder = value as Folder; 
            if (folder != null)
            {
                return folder.Folders1.Where(f => f.ParentFolderID != null && f.ParentFolderID == folder.FolderID).OrderBy(f => f.FolderID);
            }
          
            var folders = value as List<Folder>;
            if (folders != null)
            {
                return folders.Where(f => f.ParentFolderID == null).OrderBy(f=>f.FolderID);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
