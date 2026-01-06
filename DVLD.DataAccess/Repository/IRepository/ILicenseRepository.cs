using DVLD.Models;
using DVLD.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface ILicenseRepository:IRepository<License>
    {
        Task AddAsync(License license);
        Task<bool> IssueLicenseFirstTimeAsync(LicenseVM LicenseVM);
        Task<License> GetLicenseByApplicationIdAsync(int? ApplicationId);
        Task<License> GetLicenseByIdAsync(int? Id);
        bool IsOfTypeOrdinaryClass(License License);
        bool IsExpired(License License);
        Task<License>? RenewLicenseAsync(License NewLicense, License OldLicense);
        Task<License>? ReplaceLicenseAsync(License NewLicense, License OldLicense);
        Task DeactivateOldLicenseAsync(License OldLicense);

        short GetActiveLicensesCountByDriverId(int DriverId);
    }
}
