using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    [MetadataType(typeof(TaskMetaData))]
    public  partial class TaskTable
    {
    }


    public class TaskMetaData
    {

        public int TaskId { get; set; }

        [Display(Name ="ASSIGN TASK TO")]
        public int UserId { get; set; }
        public string TaskName { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Task Feedback")]
        [MinLength(6, ErrorMessage = "6 characterss minimum, please try again")]
        public string FeedBack { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "BodyContent")]
        [MinLength(6, ErrorMessage = "6 characterss minimum, please try again")]
        public string BodyContent { get; set; }
        [Display(Name = "Task Status ")]
        public bool TaskStatus { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime TastStartTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime TaskEndTime { get; set; }
        public int UnitId { get; set; }


    }
}