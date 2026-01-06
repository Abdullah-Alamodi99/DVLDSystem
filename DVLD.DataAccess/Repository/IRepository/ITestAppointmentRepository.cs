using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface ITestAppointmentRepository:IRepository<TestAppointment>
    {
        Task AddAsync(TestAppointment TestAppointment);
        void Update(TestAppointment TestAppointment);
        void Lock(TestAppointment TestAppointment);
        Task<TestAppointment >GetTestAppointmentForAppAsync(int LDLApplicationId, int TestTypeId);
        bool IsActiveAppointment(TestAppointment TestAppointment);
        void UpdateRetakeTestApplicationOnFailure(TestAppointment TestAppointment, bool HasPassedTest);
    }
}
