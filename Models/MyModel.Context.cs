﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMS.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TMSEntities1 : DbContext
    {
        public TMSEntities1()
            : base("name=TMSEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GeneralToDoListTable> GeneralToDoListTables { get; set; }
        public virtual DbSet<PersonalToDoListTable> PersonalToDoListTables { get; set; }
        public virtual DbSet<TaskTable> TaskTables { get; set; }
        public virtual DbSet<UnitTable> UnitTables { get; set; }
        public virtual DbSet<RolesTable> RolesTables { get; set; }
        public virtual DbSet<UsersTable> UsersTables { get; set; }
        public virtual DbSet<UsersRolesTable> UsersRolesTables { get; set; }
        public virtual DbSet<UserReportsTable> UserReportsTables { get; set; }
        public virtual DbSet<UnitAdminTable> UnitAdminTables { get; set; }
    }
}
