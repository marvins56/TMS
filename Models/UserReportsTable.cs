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

    public partial class UserReportsTable
    {
        public int Reportid { get; set; }
        [Display(Name = "Submitting To:")]
        public int Userid { get; set; }

        [Display(Name = "Task Reference")]
        public int TaskId { get; set; }
        public string Report { get; set; }
        [Display(Name = "Report Title ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Title field required")]
        [MaxLength(10, ErrorMessage = "20 characterss max, please try again")]
        public string Title { get; set; }
    
        public virtual TaskTable TaskTable { get; set; }
        public virtual UsersTable UsersTable { get; set; }
    }
}
