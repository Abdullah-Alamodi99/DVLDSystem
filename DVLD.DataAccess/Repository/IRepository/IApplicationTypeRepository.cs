using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IApplicationTypeRepository:IRepository<ApplicationType>
    {
        void Add(ApplicationType applicationType);
        void Update(ApplicationType applicationType);
    }
}
