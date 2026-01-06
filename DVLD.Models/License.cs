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
    public class License
    {
        public enum enIssueReason
        {
            NewLicense = 1,
            RenewLicense = 2,
            ReplaceForDamage = 3,
            ReplaceForLost = 4
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime IssueDate { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public float PaidFees { get; set; }
        [StringLength(500, ErrorMessage = "يجب ان لا تتعدى الملاحظات 500 حرف")]
        public string? Notes { get; set; }
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public byte IssueReason { get; set; }

        [Required]
        public int ApplicationId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        [ValidateNever]
        public Application Application { get; set; }

        [Required]
        public int DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        [ValidateNever]
        public Driver Driver { get; set; }

        [Required]
        public int LicenseClassId { get; set; }
        [ForeignKey(nameof(LicenseClassId))]
        [ValidateNever]
        public LicenseClass LicenseClass { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [NotMapped]
        public bool IsDetained { get; set; }

    }
}
