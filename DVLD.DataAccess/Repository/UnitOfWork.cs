

using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ITestTypeRepository TestTypes { get; set; }
        public IApplicationTypeRepository ApplicationTypes { get; set; }
        public ICountryRepository Countries { get; set; }
        public IPersonRepository People { get; set; }
        public IApplicationUserRepository Users { get; set; }
        public IApplicationRepository Applications { get; set; }
        public ILicenseClassRepository LicenseClasses { get; set; }
        public ILocalDrivingLicenseApplicationRepository LocalDrivingLicenseApplications { get; set; }
        public ITestAppointmentRepository TestAppointments { get; set; }
        public ITestRepository Tests { get; set; }
        public ILicenseRepository Licenses { get; set; }
        public IDriverRepository Drivers { get; set; }
        public IInternationalLicenseRepository InternationalLicenses { get; set; }
        public IDetainedLicenseRepository DetainedLicenses { get; set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            TestTypes = new TestTypeRepository(db);
            ApplicationTypes = new ApplicationTypeRepository(db);
            Countries = new CountryRepository(db);
            People = new PersonRepository(db);
            Users = new ApplicationUserRepository(db);
            Applications = new ApplicationRepository(db);
            LicenseClasses = new LicenseClassRepository(db);
            LocalDrivingLicenseApplications = new LocalDrivingLicenseApplicationRepository(db);
            TestAppointments = new TestAppointmentRepository(db);
            Tests = new TestRepository(db);
            Drivers = new DriverRepository(db);
            InternationalLicenses = new InternationalLicenseRepository(db);
            DetainedLicenses = new DetainedLicenseRepository(db);
            Licenses = new LicenseRepository(db, Drivers, Applications);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
