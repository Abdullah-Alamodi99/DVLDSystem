using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class TestType
    {
        public enum enTestType { VisionTest = 1, WrittenTest = 2, StreetTest = 3 };

        [NotMapped]
        public enTestType EnTestType { get; set; }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(100, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string Title { get; set; }

        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(500, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "الرسوم مطلوبة")]
        public float Fees { get; set; }
    }
}
