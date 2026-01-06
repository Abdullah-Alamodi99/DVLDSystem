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
    public class TestAppointmentRepository : Repository<TestAppointment>, ITestAppointmentRepository
    {
        private readonly ApplicationDbContext _db;
        public TestAppointmentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task AddAsync(TestAppointment TestAppointment)
        {
            await _db.TestAppointments.AddAsync(TestAppointment);
        }

        public void Update(TestAppointment TestAppointment)
        {
            _db.TestAppointments.Update(TestAppointment);
        }

        public void Lock(TestAppointment TestAppointment)
        {
            TestAppointment.IsLocked = true;
            Update(TestAppointment);
        }

        public async Task<TestAppointment> GetTestAppointmentForAppAsync(int LDLApplicationId, int TestTypeId)
        {
            var TestAppointmentFromDb = await _db.TestAppointments.Where(
               t => t.LocalDrivingLicenseApplicationId == LDLApplicationId && t.TestTypeId == TestTypeId)
               .OrderByDescending(t => t.Id)
               .Select(t => new TestAppointment
               {
                   Id = t.Id,
                   IsLocked = t.IsLocked
               }).FirstOrDefaultAsync();

            return TestAppointmentFromDb;

        }

        public bool IsActiveAppointment(TestAppointment TestAppointment)
        {
            return !TestAppointment.IsLocked;
        }

        public void UpdateRetakeTestApplicationOnFailure(TestAppointment TestAppointment, bool HasPassedTest)
        {
            if (!HasPassedTest)
                TestAppointment.RetakeTestApplicationId = TestAppointment.LocalDrivingLicenseApplicationId;
        }
    }
}
