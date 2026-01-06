using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models.ViewModels
{
    public class LicenseVM
    {
        [ValidateNever]
        public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; }
        [ValidateNever]
        public License License { get; set; }

        [ValidateNever]
        public IEnumerable<License> Licenses { get; set; }

        [ValidateNever]
        public IEnumerable<InternationalLicense> InternationalLicenses { get; set; }

        [ValidateNever]
        public Person Person { get; set; }
    }
}
