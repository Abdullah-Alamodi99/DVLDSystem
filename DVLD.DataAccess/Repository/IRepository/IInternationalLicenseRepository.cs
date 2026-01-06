using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IInternationalLicenseRepository:IRepository<InternationalLicense>
    {
        Task IssueAsync(InternationalLicense License);
        Task<bool> HasActiveInternationalLicenseByDriverIdAsync(int DriverId);
    }
}
