using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITestTypeRepository TestTypes { get; }
        IApplicationTypeRepository ApplicationTypes { get; }
        ICountryRepository Countries { get; }
        IPersonRepository People { get; }
        IApplicationUserRepository Users { get; }
        IApplicationRepository Applications { get; }
        ILicenseClassRepository LicenseClasses { get; }
        ILocalDrivingLicenseApplicationRepository LocalDrivingLicenseApplications{ get; }
        ITestAppointmentRepository TestAppointments { get; }
        ITestRepository Tests { get; }
        ILicenseRepository Licenses { get; }
        IDriverRepository Drivers { get; }
        IInternationalLicenseRepository InternationalLicenses { get; }
        IDetainedLicenseRepository DetainedLicenses { get; }
        Task<bool> SaveAsync();
    }
}
