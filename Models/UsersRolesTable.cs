//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class UsersRolesTable
    {
        public int UsersRolesID { get; set; }
        public int Userid { get; set; }
        public Nullable<int> RolesId { get; set; }
    
        public virtual RolesTable RolesTable { get; set; }
        public virtual UsersTable UsersTable { get; set; }
    }
}
