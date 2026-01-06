using DVLD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Repository.IRepository
{
    public interface IDetainedLicenseRepository:IRepository<DetainedLicense>
    {
        Task DetainAsync(DetainedLicense License);
        Task <DetainedLicense>? ReleaseAsync(DetainedLicense License);
        Task<bool> IsLicenseDetainedAsync(License License);
        Task<DetainedLicense> GetDetainedLicenseByLicenseIdAsync(int LicenseId);
        Task<DetainedLicense> GetDetainedLicenseByIdAsync(int? LicenseId);
    }
}
