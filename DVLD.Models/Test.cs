using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب تحديد نتيجة الفحص")]
        public bool TestResult { get; set; }

        [StringLength(500, ErrorMessage = "الملاحضات يجب ان لا تتعدى 500 حرف")]
        public string? Notes { get; set; }

        [Required]
        public int TestAppointmentId { get; set; }
        [ForeignKey(nameof(TestAppointmentId))]
        public TestAppointment TestAppointment { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser User { get; set; }
    }
}
