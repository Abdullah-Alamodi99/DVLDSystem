using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Utility
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_LicenseOfficer = "LicenseOfficer";
        public const string Role_Examiner = "   ";
        public const string Role_Employee = "Employee";
        public const string Roles_Admin_LicenseOfficer = Role_Admin + "," + Role_LicenseOfficer;
    }
}
