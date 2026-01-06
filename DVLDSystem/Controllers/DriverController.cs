using DVLD.DataAccess.Repository;
using DVLD.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DVLD.Models.ViewModels;
using DVLD.Models;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DriverController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PersonLicensesHistory(int? PersonId, int? DriverId)
        {
            if (!PersonId.HasValue && !DriverId.HasValue)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الشخص");

            if (DriverId.HasValue)
            {
                LicenseVM LicenseVM = new()
                {
                    Licenses = await _unitOfWork.Licenses.GetAllAsync(filter: l => l.DriverId == DriverId,
                    includeProperties: "Driver,LicenseClass,Application"),

                    InternationalLicenses = await _unitOfWork.InternationalLicenses.GetAllAsync(filter: il => il.DriverId == DriverId),
                    Person = await _unitOfWork.People.GetAsync(p => p.Id == _unitOfWork.Drivers.GetPersonIdByDriverId(DriverId), includeProperties: "Country")
                };
                return View(LicenseVM);
            }
            else
            {
                int driverId = await _unitOfWork.Drivers.GetDriverIdByPersonIdAsync(PersonId);
                LicenseVM LicenseVM = new()
                {
                    Licenses = await _unitOfWork.Licenses.GetAllAsync(l => l.DriverId == driverId,
                        includeProperties: "Driver,LicenseClass,Application"),
                    InternationalLicenses = await _unitOfWork.InternationalLicenses.GetAllAsync(filter: il => il.DriverId == driverId),
                    Person = await _unitOfWork.People.GetAsync(p => p.Id == PersonId, includeProperties: "Country")

                };

                return View(LicenseVM);
            }
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var driversList = await _unitOfWork.Drivers.GetAllDriversWithLicenseCountAsync();
            return Json(new { data = driversList });
        }

        #endregion
    }
}
