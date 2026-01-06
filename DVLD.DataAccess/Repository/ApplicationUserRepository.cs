using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task AddAsync(ApplicationUser User)
        {
            await _db.ApplicationUsers.AddAsync(User);
        }

        public async Task<bool> IsUserExistForPersonIDAsync(int Id)
        {
            return await _db.ApplicationUsers.AnyAsync(x => x.PersonId == Id);
        }

        public void Update(ApplicationUser User)
        {
            _db.ApplicationUsers.Update(User);
        }

        public void ActivateDeactivateUserAccount(ApplicationUser User)
        {
            User.IsActive = !User.IsActive;
        }
    }
}
