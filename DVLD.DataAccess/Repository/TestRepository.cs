using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        private readonly ApplicationDbContext _db;

        public TestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task AddAsync(Test Test)
        {
            await _db.AddAsync(Test);

        }


        public async Task<bool> PersonPassTestAsync(int TestAppointmentId)
        {
            if (TestAppointmentId == -1)
                return false;

            var TestFromDb = await _db.Tests.OrderByDescending(t => t.TestAppointmentId)
                .FirstOrDefaultAsync(t => t.TestAppointmentId == TestAppointmentId);
            return TestFromDb != null && TestFromDb.TestResult == true;
        }
        public async Task<int> PassedTestsCountAsync(int LDLApplicationId)
        {
            return await _db.Tests.Include(t => t.TestAppointment)
                .ThenInclude(t => t.LocalDrivingLicenseApplication)
                .Where(t => t.TestResult == true &&
                t.TestAppointment.LocalDrivingLicenseApplicationId == LDLApplicationId).CountAsync();
        }

        public async Task<bool> PersonPassTestAsync(int LDLApplicationId, int TestTypeId)
        {
            var TestAppointment = _db.TestAppointments.Where(
                t => t.LocalDrivingLicenseApplicationId == LDLApplicationId && t.TestTypeId == TestTypeId).
                OrderByDescending(t => t.Id).FirstOrDefault();

            if (TestAppointment != null)
                return await PersonPassTestAsync(TestAppointment.Id);
            return false;
        }
    }
}
