using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class LocalDrivingLicenseApplicationRepository : Repository<LocalDrivingLicenseApplication>, ILocalDrivingLicenseApplicationRepository
    {
        private readonly ApplicationDbContext _db;
        public LocalDrivingLicenseApplicationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task AddAsync(LocalDrivingLicenseApplication LocalDrivingLicenseApplication)
        {
            await _db.LocalDrivingLicenseApplications.AddAsync(LocalDrivingLicenseApplication);
        }
        public void Update(LocalDrivingLicenseApplication LocalDrivingLicenseApplication)
        {
            _db.LocalDrivingLicenseApplications.Update(LocalDrivingLicenseApplication);
        }
        public void Delete(LocalDrivingLicenseApplication LocalDrivingLicenseApplication)
        {
            _db.Remove(LocalDrivingLicenseApplication);
        }
        public void Cancel(LocalDrivingLicenseApplication LocalDrivingLicenseApplication)
        {
            LocalDrivingLicenseApplication.ApplicationStatus = (byte)LocalDrivingLicenseApplication.enStatus.Canceled;
            LocalDrivingLicenseApplication.LastStatusDate = DateTime.Now;
            Update(LocalDrivingLicenseApplication);
        }
        public async Task<bool> HasSameLicenseClassApplicationAsync(int PersonId, int LicenseClassId)
        {
            return await _db.LocalDrivingLicenseApplications.AnyAsync(l => l.PersonId == PersonId && l.LicenseClassId == LicenseClassId && l.ApplicationStatus != (byte)Application.enStatus.Canceled);
        }
        public async Task<bool> IsAllowedAgeForLicenseClassAsync(int PersonId, int LicenseClassId)
        {
            var LicenseClass = await _db.LicenseClasses.FirstOrDefaultAsync(l => l.Id == LicenseClassId);
            var Person = _db.People.FirstOrDefault(p => p.Id == PersonId);
            if (LicenseClass == null || Person == null)
                return false;

            return (DateTime.Now.Year - Person.DateOfBirth.Year) >= LicenseClass.MinimumAllowedAge;
        }

    }
}
