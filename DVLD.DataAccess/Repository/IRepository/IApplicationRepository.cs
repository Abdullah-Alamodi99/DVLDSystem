using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IApplicationRepository:IRepository<Application>
    {
        void Add(Application application);
        void Update(Application application);
        void SetApplicationStatusCompleted(int ApplicationId);
    }
}
