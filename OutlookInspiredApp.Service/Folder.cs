//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OutlookInspiredApp.Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class Folder
    {
        public Folder()
        {
            this.Emails = new HashSet<Email>();
            this.Folders1 = new HashSet<Folder>();
        }
    
        public int FolderID { get; set; }
        public string Name { get; set; }
        public Nullable<int> EmailClientID { get; set; }
        public Nullable<int> ParentFolderID { get; set; }
    
        public virtual EmailClient EmailClient { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<Folder> Folders1 { get; set; }
        public virtual Folder Folder1 { get; set; }
    }
}