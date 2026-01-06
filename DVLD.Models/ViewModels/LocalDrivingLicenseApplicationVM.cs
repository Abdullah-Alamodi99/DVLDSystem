using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models.ViewModels
{
    public class LocalDrivingLicenseApplicationVM
    {
        public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> LicenseClasses { get; set; }
    }
}
