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
    public class DetainedLicense
    {
        public enum enLicenseAction { Detain = 1, Release = 2 };

        [Key]
        public int Id { get; set; }

        [Required]
        public int LicenseId { get; set; }
        [ForeignKey(nameof(LicenseId))]
        [ValidateNever]
        public License License { get; set; }

        [Required]
        public DateTime DetainDate { get; set; }

        [Required]
        public float FineFees { get; set; }

        [Required]
        public string DetainedByUserId { get; set; }
        [ForeignKey(nameof(DetainedByUserId))]
        [ValidateNever]
        public ApplicationUser DetainedByUser { get; set; }

        [Required]
        public bool IsReleased { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public string? ReleasedByUserId { get; set; }
        [ForeignKey(nameof(ReleasedByUserId))]
        [ValidateNever]
        public ApplicationUser ReleasedByUser { get; set; }

        public int? ReleaseApplicationId { get; set; }
        [ForeignKey(nameof(ReleaseApplicationId))]
        [ValidateNever]
        public Application Application { get; set; }

    }
}
