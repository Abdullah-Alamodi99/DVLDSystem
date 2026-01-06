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
    public class TestTypeRepository : Repository<TestType>, ITestTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public TestTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task AddAsync(TestType testType)
        {
            await _db.TestTypes.AddAsync(testType);
        }

        public void Update(TestType testType)
        {
            _db.TestTypes.Update(testType);
        }
    }
}
