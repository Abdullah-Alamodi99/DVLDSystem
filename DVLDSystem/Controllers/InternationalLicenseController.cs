using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class InternationalLicenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InternationalLicenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Issue()
        {
            // get the current user
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId == null)
                return NotFound("يجب تسجيل الدخول");
            var CurrentUser = await _unitOfWork.Users.GetAsync(u => u.Id == userId);
            if (userId == null)
                return NotFound("يجب تسجيل الدخول");
            var IssueDate = DateTime.Now;

            var AppType = await _unitOfWork.ApplicationTypes.GetAsync(
                    a => a.Id == (int)Application.enApplicationType.NewInternationalLicense);

            InternationalLicense InternationalLicense = new()
            {
                IssueDate = IssueDate,
                ExpirationDate = IssueDate.AddYears(GlobalSettings.InternationalLicenseValidityLength),
                IsActive = true,
                ApplicationDate = DateTime.Now,
                LastStatusDate = DateTime.Now,
                ApplicationTypeId = (int)Application.enApplicationType.NewInternationalLicense,
                ApplicationStatus = (byte)Application.enStatus.New,
                PaidFees = AppType.Fees,
                PersonId = CurrentUser.PersonId,
                IssuedByUserId = CurrentUser.Id,
                CreatedByUserId = CurrentUser.Id,
                User = CurrentUser,
            };
            return View(InternationalLicense);
        }

        [HttpPost]
        public async Task<IActionResult> Issue(InternationalLicense InternationalLicense)
        {
            var ValidationErrors = await ValidateInternationalLicenseAsync(InternationalLicense);
            if (ValidationErrors.Any())
            {
                TempData["ErrorMessage"] = ValidationErrors.First();
                InternationalLicense.User = await _unitOfWork.Users.GetAsync(u => u.Id == InternationalLicense.IssuedByUserId);
                return View(InternationalLicense);
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.InternationalLicenses.IssueAsync(InternationalLicense);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "تم إصدار الرخصة الدولية بنجاح";
                return RedirectToAction("Index");
            }
            return View(InternationalLicense);
        }

        public async Task<ActionResult> LicenseInfo(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound("الرخصة الدولية غير موجودة");
            var InternationalLicense = await _unitOfWork.InternationalLicenses.GetAsync(il => il.Id == Id, includeProperties: "Driver.Person");
            return View(InternationalLicense);
        }

        #region VALIDATION METHODS

        public async Task<List<string>> ValidateInternationalLicenseAsync(InternationalLicense InternationalLicense)
        {
            var Errors = new List<string>();

            var LocalLicense = await _unitOfWork.Licenses.GetAsync(
                l => l.Id == InternationalLicense.IssuedUsingLocalLicenseId);

            if (LocalLicense == null)
            {
                Errors.Add("الرخصة المحلية غير موجودة");
                return Errors;
            }


            if (!_unitOfWork.Licenses.IsOfTypeOrdinaryClass(LocalLicense))
            {
                Errors.Add("لا يمكن إصدار الرخصة الدولية الا بوجود رخصة محلية من الفئة 3 (رخصة القيادة المحلية)");
                return Errors;
            }
            if (!LocalLicense.IsActive)
            {
                Errors.Add("الرخصة المحلية غير نشطة لذلك لا يمكن إصدار رخصة دولية");
                return Errors;
            }
            if (LocalLicense.IsDetained)
            {
                Errors.Add("الرخصة المحلية محجوزة لذلك لا يمكن إصدار رخصة دولية");
                return Errors;
            }
            if (await _unitOfWork.InternationalLicenses.HasActiveInternationalLicenseByDriverIdAsync(InternationalLicense.DriverId))
            {
                Errors.Add("الشخص بالفعل لديه رخصة قيادة دولية نشطة");
                return Errors;
            }
            return Errors;
        }

        #endregion

        #region API CALLS

        public async Task<IActionResult> GetAll()
        {
            var InternationalLicenses = await _unitOfWork.InternationalLicenses.GetAllAsync(includeProperties: "Driver.Person");
            return Json(new { data = InternationalLicenses });
        }

        #endregion
    }
}
