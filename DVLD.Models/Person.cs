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
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب أدخال الرقم الوطني")]
        [RegularExpression(@"^[12]\d{9}$", ErrorMessage = "الرقم الوطني غير صحيح، يجب أن يتكون من 10 أرقام ويبدأ بالرقم 2")]
        public string NationalNo { get; set; }

        [Required(ErrorMessage = "الأسم الأول مطلوب")]
        [StringLength(20, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "الأسم الثاني مطلوب")]
        [StringLength(20, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string SecondName { get; set; }

        [StringLength(20, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string? ThirdName { get; set; }

        [Required(ErrorMessage = "الأسم الاخير مطلوب")]
        [StringLength(20, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        public DateTime DateOfBirth { get; set; } = new DateTime(1900, 1, 1);

        [Required(ErrorMessage = "الجنس مطلوب")]
        public byte Gender { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(400, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string Address { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^7\d{8}$", ErrorMessage = "رقم الجوال غير صحيح، يجب أن يبدأ بالرقم 7 ويتكون من 9 أرقام")]

        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "عنوان البريد الإلكتروني غير صالح")]
        [StringLength(50, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "يجب أن يكون طول {0} بين {2} و {1} حرف")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "اختيار الدولة مطلوب")]
        public int CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        [ValidateNever]
        public Country Country { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + SecondName + " " + ThirdName + " " + LastName;
            }
        }

        [NotMapped]
        public bool IsPersonExitForUser { get; set; }
    }
}
