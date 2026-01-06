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
    public class DriverRepository : Repository<Driver>, IDriverRepository
    {
        private readonly ApplicationDbContext _db;
        public DriverRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Add(Driver Driver)
        {
            _db.Drivers.Add(Driver);
        }

        public async Task CreateDriverAsync(Driver Driver)
        {
            await _db.Drivers.AddAsync(Driver);
        }

        public IEnumerable<InternationalLicense> DriverInternationalLicenses(int? DriverId)
        {
            return _db.InternationalLicenses.Where(l => l.DriverId == DriverId);
        }

        public IEnumerable<License> DriverLocalLicenses(int? DriverId)
        {
            return _db.Licenses.Where(l => l.DriverId == DriverId);
        }

        public async Task<int> GetDriverIdByPersonIdAsync(int? personId)
        {
            var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.PersonId == personId);
            return driver?.Id ?? -1;
        }

        public int GetPersonIdByDriverId(int? DriverId)
        {
            var Driver = _db.Drivers.FirstOrDefault(d => d.Id == DriverId);
            return Driver == null ? 0 : Driver.PersonId;
        }
        public async Task<IEnumerable<object>> GetAllDriversWithLicenseCountAsync()
        {
            var query =
                from d in _db.Drivers
                join p in _db.People on d.PersonId equals p.Id
                join l in _db.Licenses.Where(x => x.IsActive) on d.Id equals l.DriverId into licenseGroup
                select new
                {
                    d.Id,
                    d.PersonId,
                    p.NationalNo,
                    FullName = p.FullName,
                    d.CreatedDate,
                    ActiveLicensesCount = licenseGroup.Count()
                };

            return await query
                .AsNoTracking()
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync<object>();
        }
    }
}
