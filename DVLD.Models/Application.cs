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
    public class Application
    {
        public enum enApplicationType
        {
            NewDrivingLicense = 1, RenewDrivingLicense = 2, ReplaceLostDrivingLicense = 3,
            ReplaceDamagedDrivingLicense = 4, ReleaseDetainedDrivingLicense = 5, NewInternationalLicense = 6, RetakeTest = 7
        };
        public enum enStatus { New = 1, Canceled = 2, Completed = 3 };
        [Key]
        [ValidateNever]
        public int Id { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }
        [Required]
        public byte ApplicationStatus { get; set; }
        [Required]
        public DateTime LastStatusDate { get; set; }
        [Required]
        public float PaidFees { get; set; }
        [Required]
        public int PersonId { get; set; }
        [ForeignKey(nameof(PersonId))]
        [ValidateNever]
        public Person Person { get; set; }
        [Required]
        public int ApplicationTypeId { get; set; }
        [ForeignKey(nameof(ApplicationTypeId))]
        [ValidateNever]
        public ApplicationType ApplicationType { get; set; }
        [Required]
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [NotMapped]
        public string StatusAsString
        {
            get
            {
                switch (ApplicationStatus)
                {
                    case 1:
                        return "جديد";
                    case 2:
                        return "ملغي";
                    case 3:
                        return "مكتمل";
                    default:
                        return "غير معروف";
                }
            }
        }

    }
}
