using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class LocalDrivingLicenseApplication : Application
    {
        [Required(ErrorMessage = "من فضلك قم بتحديد فئة الرخصة")]
        public int LicenseClassId { get; set; }
        [ForeignKey(nameof(LicenseClassId))]
        [ValidateNever]
        public LicenseClass LicenseClass { get; set; }

    }
}
