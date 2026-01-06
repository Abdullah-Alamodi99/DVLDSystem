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
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task AddAsync(Person person)
        {
            await _db.People.AddAsync(person);
        }

        public async Task<bool> IsNationalNoExistAsync(string NationalNo)
        {
            return await _db.People.AnyAsync(p => p.NationalNo == NationalNo);
        }

        public void Update(Person person)
        {
            _db.People.Update(person);
        }
    }
}
