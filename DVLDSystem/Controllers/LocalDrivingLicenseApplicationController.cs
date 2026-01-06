using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using Application = DVLD.Models.Application;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class LocalDrivingLicenseApplicationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocalDrivingLicenseApplicationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddEdit(int? Id)
        {
            LocalDrivingLicenseApplicationVM? localDrivingLicenseApplicationVM = new();

            if (Id == null || Id == 0)
            {
                // add mode
                localDrivingLicenseApplicationVM = await _FillAddNewApplicationDataAsync();
                if (localDrivingLicenseApplicationVM == null)
                    return NotFound("لا توجد بيانات متوفرة لطلب رخصة قيادة محلية");
                return View(localDrivingLicenseApplicationVM);
            }
            else
            {
                // update mode

                localDrivingLicenseApplicationVM.LicenseClasses = await GetLicenseClassesAsync();

                localDrivingLicenseApplicationVM.LocalDrivingLicenseApplication = await _unitOfWork.LocalDrivingLicenseApplications
                    .GetAsync(u => u.Id == Id, includeProperties: "Person.Country,User,LicenseClass");
                if (localDrivingLicenseApplicationVM.LocalDrivingLicenseApplication == null)
                    return NotFound();

                return View(localDrivingLicenseApplicationVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddEdit(LocalDrivingLicenseApplicationVM LDLAppVM)
        {
            if (await _unitOfWork.LocalDrivingLicenseApplications.
                HasSameLicenseClassApplicationAsync(LDLAppVM.LocalDrivingLicenseApplication.PersonId,
                LDLAppVM.LocalDrivingLicenseApplication.LicenseClassId))
            {
                TempData["ErrorMessage"] = "الشخص لديه طلب او رخصة من نفس الفئة, أختر فئة رخصة أخرى";
                ModelState.AddModelError("LocalDrivingLicenseApplication.LicenseClassId", "الشخص لديه طلب او رخصة من نفس الفئة, أختر فئة رخصة أخرى");
            }

            if (!await _unitOfWork.LocalDrivingLicenseApplications.IsAllowedAgeForLicenseClassAsync(
                LDLAppVM.LocalDrivingLicenseApplication.PersonId,
                LDLAppVM.LocalDrivingLicenseApplication.LicenseClassId))
            {
                TempData["ErrorMessage"] = "سن الشخص أقل من السن المسموح به لفئة الرخصة التي تم أختيارها";
                ModelState.AddModelError("LocalDrivingLicenseApplication.LicenseClassId", "سن الشخص أقل من السن المسموح به لفئة الرخصة التي تم أختيارها");
            }

            if (ModelState.IsValid)
            {
                string Message = "";
                if (LDLAppVM.LocalDrivingLicenseApplication.Id == 0)
                {
                    // add mode
                    await _unitOfWork.LocalDrivingLicenseApplications.AddAsync(LDLAppVM.LocalDrivingLicenseApplication);
                    Message = "تم أضافة الطلب بنجاح";
                }
                else
                {
                    _unitOfWork.LocalDrivingLicenseApplications.Update(LDLAppVM.LocalDrivingLicenseApplication);
                    Message = "تم تعديل الطلب بنجاح";
                }
                await _unitOfWork.SaveAsync();
                TempData["success"] = Message;
                return RedirectToAction("Index");

            }
            else
            {
                if (LDLAppVM.LocalDrivingLicenseApplication.Id == 0)
                {

                    LDLAppVM = await _FillAddNewApplicationDataAsync();
                }
                else
                {
                    LDLAppVM.LicenseClasses = await GetLicenseClassesAsync();
                    LDLAppVM.LocalDrivingLicenseApplication = await _unitOfWork.LocalDrivingLicenseApplications
                    .GetAsync(u => u.Id == LDLAppVM.LocalDrivingLicenseApplication.Id,
                    includeProperties: "Person,User,LicenseClass,Person.Country");
                }
            }
            return View(LDLAppVM);
        }
        public async Task<IActionResult> Details(int id)
        {
            var LDLApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l => l.Id == id, includeProperties: "Person,LicenseClass,ApplicationType,User");
            if (LDLApplication == null)
                return NotFound($"لا توجد بيانات للطلب بالرقم {id}");
            return View(LDLApplication);
        }


        #region Normal Functions
        private async Task<LocalDrivingLicenseApplicationVM>? _FillAddNewApplicationDataAsync()
        {
            // get the current user
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                var AppType = await _unitOfWork.ApplicationTypes.GetAsync(u => u.Id == (int)Application.enApplicationType.NewDrivingLicense);
                LocalDrivingLicenseApplicationVM localDrivingLicenseApplicationVM = new()
                {
                    LicenseClasses = await GetLicenseClassesAsync(),

                    LocalDrivingLicenseApplication = new LocalDrivingLicenseApplication()
                    {
                        ApplicationDate = DateTime.Now,
                        LastStatusDate = DateTime.Now,
                        PaidFees = AppType.Fees,
                        User = await _unitOfWork.Users.GetAsync(u => u.Id == userId),
                        CreatedByUserId = userId,
                        ApplicationStatus = (byte)Application.enStatus.New,
                        ApplicationTypeId = (int)Application.enApplicationType.NewDrivingLicense
                    }
                };
                return localDrivingLicenseApplicationVM;
            }
            return null;
        }
        private async Task<IEnumerable<SelectListItem>> GetLicenseClassesAsync()
        {
            var LicenseClasses = await _unitOfWork.LicenseClasses.GetAllAsync();
            return LicenseClasses.Select(
                        u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
        }
        #endregion



        #region API CALLS   
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 1. Load applications with related data
            var LDLApplications = await _unitOfWork.LocalDrivingLicenseApplications.GetAllAsync(
                includeProperties: "LicenseClass,Person"
            );

            // 2. Load all tests related to these applications in one query
            var appIds = LDLApplications.Select(a => a.Id).ToList();
            var allTests = await _unitOfWork.Tests.GetAllAsync(t => appIds.Contains(t.TestAppointment.LocalDrivingLicenseApplicationId),
                includeProperties: "TestAppointment");

            // 3. Project data efficiently in memory
            var result = LDLApplications.Select(l =>
            {
                var testsForApp = allTests.Where(t => t.TestAppointment.LocalDrivingLicenseApplicationId == l.Id);

                bool passedVision = testsForApp.Any(t => t.TestAppointment.TestTypeId == (int)TestType.enTestType.VisionTest && t.TestResult);
                bool passedWritten = testsForApp.Any(t => t.TestAppointment.TestTypeId == (int)TestType.enTestType.WrittenTest && t.TestResult);
                bool passedStreet = testsForApp.Any(t => t.TestAppointment.TestTypeId == (int)TestType.enTestType.StreetTest && t.TestResult);

                return new
                {
                    Id = l.Id,
                    LicenseClassName = l.LicenseClass.Name,
                    NationalNo = l.Person.NationalNo,
                    FullName = l.Person.FullName,
                    ApplicationDate = l.ApplicationDate,
                    ApplicationStatus = l.ApplicationStatus,
                    PassedTestsCount = testsForApp.Count(t => t.TestResult),
                    PersonId = l.PersonId,
                    HasPassedVisionTest = passedVision,
                    HasPassedWrittenTest = passedWritten,
                    HasPassedStreetTest = passedStreet
                };
            });

            return Json(new { data = result });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var LDLApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l => l.Id == id);
            if (LDLApplication == null)
                return Json(new { success = false, message = "حدث خطأ : لا يمكن حذف هذا الطلب" });

            _unitOfWork.LocalDrivingLicenseApplications.Delete(LDLApplication);

            if (!await _unitOfWork.SaveAsync())
                return Json(new { success = false, message = "حدث خطأ : لا يمكن حذف هذا الطلب لوجود بيانات مرتبطة به" });

            return Json(new { success = true, message = "تم حذف الطلب بنجاح" });

        }

        [HttpPut]
        public async Task<IActionResult> Cancel(int id)
        {
            var LDLApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l => l.Id == id);
            if (LDLApplication == null)
                return Json(new { success = false, message = "حدث خطأ : لا يمكن إلغاء الطلب" });

            _unitOfWork.LocalDrivingLicenseApplications.Cancel(LDLApplication);

            if (!await _unitOfWork.SaveAsync())
                return Json(new { success = false, message = "حدث خطأ : لا يمكن إلغاء الطلب" });

            return Json(new { success = true, message = "تم إلغاء الطلب بنجاح" });
        }

        #endregion
    }
}
