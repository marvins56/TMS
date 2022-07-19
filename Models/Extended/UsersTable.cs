using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMS.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class UsersTable
    {
       
        public string ConfirmPassword { get; set; }
    }
    public class UserMetaData
    {
        [Display(Name = "UserID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UnitID field required")]
        public int Userid { get; set; }
        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username field required")]
        public string Username { get; set; }


        [Display(Name = "Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Email Address field required")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }

        [Display(Name = "Date Of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = " Date Of Birth field required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime DOB { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password field required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "6 characterss minimum, please try again")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = " Password missmatch")]
        public string ConfirmPassword { get; set; }
       




    }
}