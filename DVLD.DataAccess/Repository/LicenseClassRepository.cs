using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class LicenseClassRepository : Repository<LicenseClass>, ILicenseClassRepository
    {
        private readonly ApplicationDbContext _db;
        public LicenseClassRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
