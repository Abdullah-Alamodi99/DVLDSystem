using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models
{
    [NotMapped]
    public static class GlobalSettings
    {
        public static byte InternationalLicenseValidityLength = 1;
    }
}
