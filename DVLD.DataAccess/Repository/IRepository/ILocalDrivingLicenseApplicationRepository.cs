using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface ILocalDrivingLicenseApplicationRepository:IRepository<LocalDrivingLicenseApplication>
    {
        Task AddAsync(LocalDrivingLicenseApplication LocalDrivingLicenseApplication);
        void Update(LocalDrivingLicenseApplication LocalDrivingLicenseApplication);
        void Delete(LocalDrivingLicenseApplication LocalDrivingLicenseApplication);
        public void Cancel(LocalDrivingLicenseApplication LocalDrivingLicenseApplication);
        Task<bool> HasSameLicenseClassApplicationAsync(int PersonId, int LicenseClassId);
        Task<bool> IsAllowedAgeForLicenseClassAsync(int PersonId, int LicenseClassId);
    }
}
