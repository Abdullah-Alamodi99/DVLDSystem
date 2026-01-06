using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IPersonRepository:IRepository<Person>
    {
        Task AddAsync(Person person);
        void Update(Person person); 
        Task<bool> IsNationalNoExistAsync(string NationalNo);
    }
}
