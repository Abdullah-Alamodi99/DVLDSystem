using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class TestAppointment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "تاريخ موعد الاختبار مطلوب")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "رسوم موعد الاختبار مطلوبة")]
        public float PaidFees { get; set; }
        [Required]
        public bool IsLocked { get; set; }

        [Required(ErrorMessage = "نوع الاختبار مطلوب")]
        public int TestTypeId { get; set; }

        [ForeignKey(nameof(TestTypeId))]
        public TestType TestType { get; set; }

        [Required]
        public int LocalDrivingLicenseApplicationId { get; set; }
        [ForeignKey(nameof(LocalDrivingLicenseApplicationId))]
        public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser User { get; set; }

        public int? RetakeTestApplicationId { get; set; }
        [ForeignKey(nameof(RetakeTestApplicationId))]
        public Application Application { get; set; }

    }
}
