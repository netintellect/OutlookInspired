﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OutlookEntities : DbContext
    {
        public OutlookEntities()
            : base("name=OutlookEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AppointmentResource> AppointmentResources { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EmailClient> EmailClients { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<ExceptionAppointment> ExceptionAppointments { get; set; }
        public DbSet<ExceptionOccurrence> ExceptionOccurrences { get; set; }
        public DbSet<ExceptionResource> ExceptionResources { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<TimeMarker> TimeMarkers { get; set; }
    }
}
