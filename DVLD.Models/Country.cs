using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "أسم الدولة مطلوب")]
        [StringLength(50, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string Name { get; set; }
    }
}
