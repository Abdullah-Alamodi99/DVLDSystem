using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class LicenseRepository : Repository<License>, ILicenseRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IDriverRepository _driverRepository;
        private readonly IApplicationRepository _applicationRepository;
        private async Task<License>? HandleLicenseRenewalAndReplacementAsync(License NewLicense, License OldLicense)
        {
            using var Transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                Application Application = NewLicense.Application;
                await _db.Applications.AddAsync(Application);
                if (await _db.SaveChangesAsync() == 0)
                {
                    await Transaction.RollbackAsync();
                    return null;
                }

                NewLicense.ApplicationId = Application.Id;
                await _db.Licenses.AddAsync(NewLicense);
                if (await _db.SaveChangesAsync() == 0)
                {
                    await Transaction.RollbackAsync();
                    return null;
                }

                await DeactivateOldLicenseAsync(OldLicense);
                await Transaction.CommitAsync();
                return NewLicense;
            }
            catch (Exception ex)
            {
                await Transaction.RollbackAsync();
                return null;
            }
        }

        public LicenseRepository(ApplicationDbContext db, IDriverRepository driverRepository, IApplicationRepository applicationRepository) : base(db)
        {
            _db = db;
            _driverRepository = driverRepository;
            _applicationRepository = applicationRepository;

        }

        public async Task AddAsync(License license)
        {
            await _db.Licenses.AddAsync(license);
        }

        public async Task<bool> IssueLicenseFirstTimeAsync(LicenseVM LicenseVM)
        {
            using var Transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                int DriverId = await _driverRepository.GetDriverIdByPersonIdAsync(LicenseVM.LocalDrivingLicenseApplication.PersonId);
                bool IsDriverExist = DriverId != -1;
                if (!IsDriverExist)
                {
                    LicenseVM.License.Driver = new()
                    {
                        PersonId = LicenseVM.LocalDrivingLicenseApplication.PersonId,
                        CreatedByUserId = LicenseVM.License.CreatedByUserId,
                        CreatedDate = DateTime.Now
                    };

                    await _driverRepository.CreateDriverAsync(LicenseVM.License.Driver);
                    if (await _db.SaveChangesAsync() == 0)
                    {
                        await Transaction.RollbackAsync();
                        return false;
                    }
                    LicenseVM.License.DriverId = LicenseVM.License.Driver.Id;
                }
                else
                {
                    LicenseVM.License.DriverId = DriverId;
                }

                await _db.Licenses.AddAsync(LicenseVM.License);
                if (await _db.SaveChangesAsync() == 0)
                {
                    await Transaction.RollbackAsync();
                    return false;
                }

                _applicationRepository.SetApplicationStatusCompleted(LicenseVM.License.ApplicationId);
                if (await _db.SaveChangesAsync() == 0)
                {
                    await Transaction.RollbackAsync();
                    return false;
                }

                Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                return false;
            }

        }

        public async Task DeactivateOldLicenseAsync(License OldLicense)
        {
            if (OldLicense != null)
            {
                OldLicense.IsActive = false;
                _db.Licenses.Update(OldLicense);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<License> GetLicenseByApplicationIdAsync(int? ApplicationId)
        {
            var License = await _db.Licenses.Include(l => l.LicenseClass).Include(l => l.Driver)
                 .Include(l => l.Application).ThenInclude(a => a.Person)
                 .FirstOrDefaultAsync(l => l.ApplicationId == ApplicationId);
            return License;
        }

        public async Task<License> GetLicenseByIdAsync(int? Id)
        {
            var License = await _db.Licenses.Include(l => l.LicenseClass).Include(l => l.Driver)
                .Include(l => l.Application).ThenInclude(a => a.Person)
                .FirstOrDefaultAsync(l => l.Id == Id);
            if (License != null)
                License.IsDetained = await _db.DetainedLicenses.AnyAsync(dl => dl.LicenseId == Id && !dl.IsReleased);
            return License;
        }

        public bool IsOfTypeOrdinaryClass(License License)
        {
            return License.LicenseClassId == 3;
        }

        public bool IsExpired(License License)
        {
            return License.ExpirationDate < DateTime.Now;
        }

        public async Task<License>? RenewLicenseAsync(License NewLicense, License OldLicense)
        {
            return await HandleLicenseRenewalAndReplacementAsync(NewLicense, OldLicense);
        }

        public async Task<License>? ReplaceLicenseAsync(License NewLicense, License OldLicense)
        {
            return await HandleLicenseRenewalAndReplacementAsync(NewLicense, OldLicense);

        }

        public short GetActiveLicensesCountByDriverId(int DriverId)
        {
            return (short)_db.Licenses.Where(l => l.DriverId == DriverId && l.IsActive).Count();
        }
    }
}
