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
    using System.ComponentModel.DataAnnotations;

    public partial class UnitAdminTable
    {
        public int UnitAdminid { get; set; }
        [Display(Name = "User Name ")]
        public int Userid { get; set; }
        [Display(Name = "User Unit ")]
        public int UnitId { get; set; }
    
        public virtual UnitTable UnitTable { get; set; }
        public virtual UsersTable UsersTable { get; set; }
    }
}
