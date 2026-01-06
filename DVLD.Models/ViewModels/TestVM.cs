using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Models.ViewModels
{
    public class TestVM
    {

        [ValidateNever]
        public Test Test { get; set; }

        [ValidateNever]
        public TestAppointment TestAppointment { get; set; }

        [ValidateNever]
        public bool HasPassedTest { get; set; }
    }
}
