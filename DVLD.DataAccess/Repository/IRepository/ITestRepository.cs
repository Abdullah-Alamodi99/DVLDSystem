using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface ITestRepository:IRepository<Test>
    {
        Task AddAsync(Test Test);
        Task<bool> PersonPassTestAsync(int TestAppointmentId);
        Task<bool> PersonPassTestAsync(int LDLApplicationId, int TestTypeId);
        Task<int> PassedTestsCountAsync(int LDLApplicationId);
    }
}
