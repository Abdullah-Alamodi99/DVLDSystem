using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository
{
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Add(Application application)
        {
            _db.Applications.Add(application);
        }


        public void Update(Application application)
        {
            _db.Applications.Update(application);
        }
        public void SetApplicationStatusCompleted(int ApplicationId)
        {
            var ApplicationFromDb = _db.Applications.FirstOrDefault(a => a.Id == ApplicationId);
            if (ApplicationFromDb != null)
            {
                ApplicationFromDb.ApplicationStatus = (byte)Application.enStatus.Completed;
                ApplicationFromDb.LastStatusDate = DateTime.Now;
                _db.Applications.Update(ApplicationFromDb);
            }
        }
    }
}
