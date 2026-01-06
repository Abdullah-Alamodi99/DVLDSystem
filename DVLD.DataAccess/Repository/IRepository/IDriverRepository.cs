using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IDriverRepository:IRepository<Driver>
    {
        void Add(Driver Driver);
        Task CreateDriverAsync(Driver Driver);
        Task<int> GetDriverIdByPersonIdAsync(int? PersonId);
        int GetPersonIdByDriverId(int? DriverId);
        IEnumerable<License> DriverLocalLicenses(int? DriverId);
        IEnumerable<InternationalLicense> DriverInternationalLicenses(int? DriverId);
        Task<IEnumerable<object>> GetAllDriversWithLicenseCountAsync();

    }
}
