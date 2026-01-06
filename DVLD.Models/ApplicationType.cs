using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="عنوان الطلب مطلوب")]
        [StringLength(150, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string Title { get; set; }
        [Required(ErrorMessage = "رسوم الطلب مطلوبة")]
        public float Fees { get; set; }
    }
}
