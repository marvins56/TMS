using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    [MetadataType(typeof(PersonalToDoListMetaData))]
    public partial class PersonalToDoListTable
    {
    }


    public class PersonalToDoListMetaData
    {
        [Key]
        public int PToDoListId { get; set; }

        [Display(Name = "TASK NAME")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "TASK NAME field required")]
        public string UserName { get; set; }

        [Display(Name = "DESCRIPTION")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "DESCRIPTION field required")]
        [DataType(DataType.MultilineText)]
        public string TaskName { get; set; }
        [Display(Name = "TASK RESULTS / FEEDBACK")]
        [DataType(DataType.MultilineText)]
        public string TaskFeedback { get; set; }
    }

}