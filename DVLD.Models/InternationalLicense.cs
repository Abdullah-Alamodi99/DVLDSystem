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
    public class InternationalLicense : Application
    {

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        [ValidateNever]
        public Driver Driver { get; set; }

        [Required]
        public int IssuedUsingLocalLicenseId { get; set; }
        [ForeignKey(nameof(IssuedUsingLocalLicenseId))]
        [ValidateNever]
        public License LocalLicense { get; set; }

        [Required]
        public string IssuedByUserId { get; set; }
        [ForeignKey(nameof(IssuedByUserId))]
        [ValidateNever]
        public ApplicationUser User { get; set; }
    }
}
