using Microsoft.AspNetCore.Identity;
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
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "يجب ان يكون المستخدم هو شخص معرف من قبل في النظام")]
        public int PersonId { get; set; }
        [ForeignKey(nameof(PersonId))]
        [ValidateNever]
        public Person Person { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public string Role { get; set; }

    }
}
