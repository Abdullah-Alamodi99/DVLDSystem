using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using DVLD.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DVLDSystem.Controllers
{
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Examiner}")]
    public class TestController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Take(int TestAppointmentId)
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (UserId == null)
                return NotFound("يجب تسجيل الدخول");


            var TestAppointmentFromDb = await _unitOfWork.TestAppointments.GetAsync(t => t.Id == TestAppointmentId, includeProperties: "LocalDrivingLicenseApplication.Person,LocalDrivingLicenseApplication.LicenseClass,TestType");

            if (TestAppointmentFromDb == null)
                return NotFound("تعذر تحميل بيانات موعد الفحص");


            TestVM TestVM = new()
            {
                Test = new()
                {
                    TestAppointmentId = TestAppointmentId,
                    CreatedByUserId = UserId,
                    TestResult = true
                },
                TestAppointment = TestAppointmentFromDb,
                HasPassedTest = await _unitOfWork.Tests.PersonPassTestAsync
                    (TestAppointmentFromDb.LocalDrivingLicenseApplicationId, TestAppointmentFromDb.TestTypeId)
            };

            return View(TestVM);
        }

        [HttpPost]
        public async Task<IActionResult> Take(TestVM TestVM)
        {
            var TestAppointmentFromDb = await _unitOfWork.TestAppointments.GetAsync(t => t.Id == TestVM.Test.TestAppointmentId, includeProperties: "LocalDrivingLicenseApplication.Person,LocalDrivingLicenseApplication.LicenseClass,TestType");

            if (ModelState.IsValid)
            {
                await _unitOfWork.Tests.AddAsync(TestVM.Test);
                await _unitOfWork.SaveAsync();
                _unitOfWork.TestAppointments.Lock(TestAppointmentFromDb);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "تم عمل الفحص بنجاح";
                return RedirectToAction("Index", "TestAppointment",
                    new
                    {
                        LDLAppID = TestAppointmentFromDb.LocalDrivingLicenseApplicationId,
                        TestTypeID = TestAppointmentFromDb.TestTypeId
                    });
            }
            return View(TestVM);
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> PersonPassedTest(int LDLApplicationId, int TestTypeId)
        {
            bool PassedTest = await _unitOfWork.Tests.PersonPassTestAsync(LDLApplicationId, TestTypeId);
            return Json(PassedTest);
        }
        #endregion

    }
}
