using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models.ViewModels
{
    public class TestAppointmentVM
    {
        [ValidateNever]
        public TestAppointment TestAppointment { get; set; }
        [ValidateNever]
        public List<TestAppointment> TestAppointments { get; set; }

        [ValidateNever]
        public LocalDrivingLicenseApplication LocalDrivingLicenseApplication { get; set; }

        [ValidateNever]
        public TestType TestType { get; set; }

        [ValidateNever]
        public bool HasPassedTest { get; set; }

        [ValidateNever]
        public float RetakeTestApplicationFees { get; set; }
    }
}
