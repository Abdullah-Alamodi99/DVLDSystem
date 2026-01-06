using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLDSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class DetainedLicenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetainedLicenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detain(int? DetainedLicenseId)
        {
            DetainedLicense DetainedLicense;

            if (DetainedLicenseId == null || DetainedLicenseId == 0)
            {
                DetainedLicense = await _FillDetainLicenseInfoAsync();
                ViewBag.CanPerformLicenseAction = false;

                if (DetainedLicense.DetainedByUser == null)
                    return Unauthorized("يجب تسجيل الدخول");
                return View(DetainedLicense);
            }

            DetainedLicense = await _unitOfWork.DetainedLicenses.GetAsync(dl => dl.Id == DetainedLicenseId);
            if (DetainedLicense == null)
                return NotFound("الرخصة غير موجودة");

            return View(DetainedLicense);

        }

        [HttpPost]
        public async Task<IActionResult> Detain(DetainedLicense DetainedLicense)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.DetainedLicenses.DetainAsync(DetainedLicense);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "تم حجز الرخصة بنجاح";
                return RedirectToAction("LicenseInfoById", "License", new { Id = DetainedLicense.LicenseId });
            }
            ViewBag.CanPerformLicenseAction = false;
            return View(DetainedLicense);
        }


        public async Task<IActionResult> Release(int? DetainedLicenseId)
        {
            var CurrentUser = await _unitOfWork.Users.GetAsync(u => u.Id == User.GetCurrentUserId());
            if (CurrentUser == null)
                return Unauthorized("يجب تسجيل الدخول");

            DetainedLicense DetainedLicense;

            if (DetainedLicenseId == null || DetainedLicenseId == 0)
            {
                var appType = await _unitOfWork.ApplicationTypes.GetAsync(
                            a => a.Id == (int)Application.enApplicationType.ReleaseDetainedDrivingLicense);
                DetainedLicense = new()
                {
                    ReleasedByUserId = CurrentUser.Id,
                    ReleasedByUser = CurrentUser,
                    Application = new()
                    {
                        PaidFees = appType.Fees
                    }
                };
                ViewBag.CanPerformLicenseAction = false;
                return View(DetainedLicense);
            }

            DetainedLicense = await _unitOfWork.DetainedLicenses.GetDetainedLicenseByIdAsync(DetainedLicenseId);

            if (DetainedLicense == null)
                return NotFound("الرخصة غير موجودة");
            DetainedLicense = await _FillReleaseLicenseInfoAsync(DetainedLicense);
            ViewBag.CanPerformLicenseAction = true;

            return View(DetainedLicense);

        }

        [HttpPost]
        public async Task<IActionResult> Release(DetainedLicense DetainedLicense)
        {
            if (ModelState.IsValid)
            {
                var ReleasedLicense = await _unitOfWork.DetainedLicenses.ReleaseAsync(DetainedLicense);
                if (ReleasedLicense != null)
                {
                    TempData["success"] = "تم فك حجز الرخصة";
                    return RedirectToAction("LicenseInfoById", "License", new { Id = DetainedLicense.LicenseId });
                }
                TempData["ErrorMessage"] = "حدث خطأ عند محاولة فك حجز الرخصة";
            }
            ViewBag.CanPerformLicenseAction = false;
            TempData["ErrorMessage"] = "البيانات المدخلة غير صحيحة";
            return View(DetainedLicense);
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var detainedLicenses = await _unitOfWork.DetainedLicenses
                .GetAllAsync(selector: dl => new
                {
                    dl.Id,
                    dl.LicenseId,
                    dl.DetainDate,
                    dl.IsReleased,
                    dl.FineFees,
                    dl.ReleaseDate,
                    dl.ReleaseApplicationId,
                    NationalNo = dl.License.Application.Person.NationalNo,
                    FullName = dl.License.Application.Person.FullName,
                    PersonId = dl.License.Application.Person.Id
                },
                includeProperties: "License.Application.Person");

            return Json(new { data = detainedLicenses });
        }

        [HttpPost]
        public async Task<ActionResult> SearchLocalLicense(int LicenseId, DetainedLicense.enLicenseAction LicenseAction)
        {
            var License = await _unitOfWork.Licenses.GetLicenseByIdAsync(LicenseId);
            DetainedLicense DetainedLicense = new();

            if (License != null)
            {
                switch (LicenseAction)
                {
                    case DetainedLicense.enLicenseAction.Detain:
                        DetainedLicense.License = License;
                        DetainedLicense = await _FillDetainLicenseInfoAsync(DetainedLicense);
                        if (DetainedLicense.DetainedByUser == null)
                            return Unauthorized("يجب تسجيل الدخول");
                        DetainedLicense.LicenseId = License.Id;
                        break;
                    case DetainedLicense.enLicenseAction.Release:
                        DetainedLicense = await _unitOfWork.DetainedLicenses.GetDetainedLicenseByLicenseIdAsync(LicenseId);

                        if (DetainedLicense != null)
                        {
                            DetainedLicense = await _FillReleaseLicenseInfoAsync(DetainedLicense);
                            if (DetainedLicense.ReleasedByUser == null)
                                return Unauthorized("يجب تسجيل الدخول");
                        }
                        else
                        {
                            DetainedLicense = new()
                            {
                                License = License,
                            };
                            DetainedLicense = await _FillReleaseLicenseInfoAsync(DetainedLicense);
                        }
                        break;
                }
                var ValidateErrors = await ValidateLicenseActionAsync(License, LicenseAction);
                if (ValidateErrors.Any())
                {
                    TempData["ErrorMessage"] = ValidateErrors.First();
                    ViewBag.CanPerformLicenseAction = false;
                }
                else
                    ViewBag.CanPerformLicenseAction = true;

            }
            else
            {
                if (LicenseAction == DetainedLicense.enLicenseAction.Detain)
                    DetainedLicense = await _FillDetainLicenseInfoAsync();
                else
                    DetainedLicense = await _FillReleaseLicenseInfoAsync();

                TempData["ErrorMessage"] = "الرخصة غير موجودة";
                ViewBag.CanPerformLicenseAction = false;
            }
            if (LicenseAction == DetainedLicense.enLicenseAction.Detain)
                return View("Detain", DetainedLicense);

            return View("Release", DetainedLicense);
        }

        #endregion

        #region NORMAL FUNCTIONS

        public async Task<List<string>> ValidateLicenseActionAsync(License License, DetainedLicense.enLicenseAction LicenseAction)
        {
            List<string> Errors = new();

            // validation for detain license action
            if (LicenseAction == DetainedLicense.enLicenseAction.Detain)
            {
                if (!License.IsActive)
                {
                    Errors.Add("لا يمكن حجز الرخصة بسبب ان الرخصة غير نشطة");
                    return Errors;
                }
                if (await _unitOfWork.DetainedLicenses.IsLicenseDetainedAsync(License))
                {
                    Errors.Add("لا يمكن حجز الرخصة بسبب ان الرخصة محجوزة بالفعل ");
                    return Errors;
                }

                return Errors;
            }

            // validation for release license action
            if (!await _unitOfWork.DetainedLicenses.IsLicenseDetainedAsync(License))
            {
                Errors.Add("لا يمكن تنفيذ العملية لان الرخصة غير محجوزة بالفعل");
                return Errors;
            }
            return Errors;
        }

        private async Task<DetainedLicense> _FillDetainLicenseInfoAsync(DetainedLicense? DetainedLicense = null)
        {
            var CurrentUser = await _unitOfWork.Users.GetAsync(u => u.Id == User.GetCurrentUserId());
            if (DetainedLicense != null)
            {
                DetainedLicense.DetainDate = DateTime.Now;
                DetainedLicense.DetainedByUserId = CurrentUser.Id;
                DetainedLicense.DetainedByUser = CurrentUser;
            }
            else
            {
                DetainedLicense = new()
                {
                    DetainDate = DateTime.Now,
                    DetainedByUserId = CurrentUser.Id,
                    DetainedByUser = CurrentUser,
                };
            }
            return DetainedLicense;
        }
        private async Task<DetainedLicense> _FillReleaseLicenseInfoAsync(DetainedLicense? DetainedLicense = null)
        {
            var CurrentUser = await _unitOfWork.Users.GetAsync(u => u.Id == User.GetCurrentUserId());
            var ApplicationType = await _unitOfWork.ApplicationTypes.GetAsync(
                            a => a.Id == (int)Application.enApplicationType.ReleaseDetainedDrivingLicense);
            if (DetainedLicense != null)
            {
                DetainedLicense.ReleaseDate = DateTime.Now;
                DetainedLicense.ReleasedByUserId = CurrentUser.Id;
                DetainedLicense.ReleasedByUser = CurrentUser;
                DetainedLicense.IsReleased = true;
                DetainedLicense.Application = await _FillApplicationInfoAsync(DetainedLicense);
            }
            else
            {
                DetainedLicense = new()
                {
                    ReleaseDate = DateTime.Now,
                    ReleasedByUserId = CurrentUser.Id,
                    ReleasedByUser = CurrentUser,
                    IsReleased = true,
                };
                DetainedLicense.Application = new()
                {
                    PaidFees = ApplicationType.Fees
                };
            }
            return DetainedLicense;
        }

        private async Task<Application> _FillApplicationInfoAsync(DetainedLicense DetainedLicense)
        {
            var ApplicationType = await _unitOfWork.ApplicationTypes.GetAsync(
                                    a => a.Id == (byte)Application.enApplicationType.ReleaseDetainedDrivingLicense);
            DetainedLicense.Application = new()
            {
                ApplicationDate = DateTime.Now,
                ApplicationStatus = (byte)Application.enStatus.Completed,
                LastStatusDate = DateTime.Now,
                PaidFees = ApplicationType.Fees,
                PersonId = DetainedLicense.License.Driver.PersonId,
                ApplicationTypeId = (int)Application.enApplicationType.ReleaseDetainedDrivingLicense,
                CreatedByUserId = DetainedLicense.ReleasedByUser.Id,
            };
            return DetainedLicense.Application;
        }

        #endregion
    }
}
