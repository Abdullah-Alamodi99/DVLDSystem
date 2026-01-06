using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface ITestTypeRepository:IRepository<TestType>
    {
        Task AddAsync(TestType testType);
        void Update(TestType testType);
    }
}
