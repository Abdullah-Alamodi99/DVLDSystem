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
    public class InternationalLicenseRepository : Repository<InternationalLicense>, IInternationalLicenseRepository
    {
        private readonly ApplicationDbContext _db;
        public InternationalLicenseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task IssueAsync(InternationalLicense License)
        {
            await _db.InternationalLicenses.AddAsync(License);
        }
        public async Task<bool> HasActiveInternationalLicenseByDriverIdAsync(int DriverId)
        {
            return await _db.InternationalLicenses.OrderByDescending(il => il.ExpirationDate)
                .AnyAsync(il => il.DriverId == DriverId && il.ExpirationDate > DateTime.Now);
        }
    }
}
