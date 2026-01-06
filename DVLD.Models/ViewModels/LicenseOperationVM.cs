using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models.ViewModels
{
    public class LicenseOperationVM
    {
        [ValidateNever]
        public License OldLicense { get; set; }
        public License NewLicense { get; set; }

        public enum enLicenseOperationType { Renew, DamagedReplacement, LostReplacement }
        [ValidateNever]
        public enLicenseOperationType OperationType { get; set; }
    }
}
