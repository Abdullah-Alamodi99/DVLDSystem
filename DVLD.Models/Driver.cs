using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class Driver
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PersonId { get; set; }
        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }
        [Required]
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser User { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
