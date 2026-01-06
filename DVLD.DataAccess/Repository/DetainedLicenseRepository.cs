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
    public class DetainedLicenseRepository : Repository<DetainedLicense>, IDetainedLicenseRepository
    {
        private readonly ApplicationDbContext _db;
        public DetainedLicenseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task DetainAsync(DetainedLicense License)
        {
            await _db.DetainedLicenses.AddAsync(License);
        }

        public async Task<DetainedLicense> GetDetainedLicenseByLicenseIdAsync(int LicenseId)
        {
            var DetainedLicense = await _db.DetainedLicenses.Include(dl => dl.DetainedByUser)
                .Include(dl => dl.License).ThenInclude(dl => dl.LicenseClass).Include(dl => dl.Application.Person)
                .FirstOrDefaultAsync(dl => dl.LicenseId == LicenseId && !dl.IsReleased);
            if (DetainedLicense != null)
                DetainedLicense.License.IsDetained = !DetainedLicense.IsReleased;
            return DetainedLicense;
        }
        public async Task<DetainedLicense> GetDetainedLicenseByIdAsync(int? DetainedLicenseId)
        {
            if (DetainedLicenseId == 0 || DetainedLicenseId == null)
                return null;

            var DetainedLicense = await _db.DetainedLicenses
                .Include(dl => dl.DetainedByUser).Include(dl => dl.Application)
                .Include(dl => dl.License)
                .ThenInclude(dl => dl.LicenseClass).Include(dl => dl.License.Application.Person)
                .Include(dl => dl.License.Driver)
                .FirstOrDefaultAsync(dl => dl.Id == DetainedLicenseId && !dl.IsReleased);
            if (DetainedLicense != null)
                DetainedLicense.License.IsDetained = !DetainedLicense.IsReleased;
            return DetainedLicense;
        }

        public async Task<bool> IsLicenseDetainedAsync(License License)
        {
            return await _db.DetainedLicenses.AnyAsync(dl => dl.LicenseId == License.Id && !dl.IsReleased);
        }

        public async Task<DetainedLicense>? ReleaseAsync(DetainedLicense License)
        {
            using var Transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var Application = License.Application;
                await _db.AddAsync(Application);
                if (_db.SaveChanges() == 0)
                {
                    await Transaction.RollbackAsync();
                    return null;
                }
                _db.DetainedLicenses.Update(License);
                if (_db.SaveChanges() == 0)
                {
                    await Transaction.RollbackAsync();
                    return null;
                }
                await Transaction.CommitAsync();
                return License;
            }
            catch (Exception ex)
            {
                await Transaction.RollbackAsync();
                return null;
            }
        }
    }
}
