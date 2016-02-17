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
    
    public partial class Email
    {
        public int EmailID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAddress { get; set; }
        public System.DateTime Received { get; set; }
        public int Status { get; set; }
        public string CarbonCopy { get; set; }
        public Nullable<int> FolderID { get; set; }
        public Nullable<int> EmailClientID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> Flag { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual EmailClient EmailClient { get; set; }
        public virtual Folder Folder { get; set; }
    }
}
