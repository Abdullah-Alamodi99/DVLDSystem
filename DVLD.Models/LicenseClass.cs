using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    public class LicenseClass
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        [Required]
        public byte MinimumAllowedAge { get; set; }
        [Required]
        public byte DefualtValidityLength { get; set; }
        [Required]
        public float Fees { get; set; }
    }
}
