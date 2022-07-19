using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMS.Models
{
    [MetadataType(typeof(UnitMetaData))]
    public partial class UnitTable
    {

    }


    public class UnitMetaData
    {
        [Display(Name = "UnitID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UnitID field required")]
        [Key]
        public int UnitId { get; set; }

        [Display(Name = "Unit Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Unit Name field required")]
        public string UnitName { get; set; }

    }
}