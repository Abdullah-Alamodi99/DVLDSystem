using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository:IRepository<ApplicationUser>
    {
        Task AddAsync(ApplicationUser User);
        void Update(ApplicationUser User);
        Task<bool> IsUserExistForPersonIDAsync(int Id);
        void ActivateDeactivateUserAccount(ApplicationUser User);
    }
}
