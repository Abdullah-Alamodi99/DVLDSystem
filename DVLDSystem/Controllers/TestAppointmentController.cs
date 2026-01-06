using DVLD.DataAccess.Repository;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using DVLD.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace DVLDSystem.Controllers
{
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Examiner}")]
    public class TestAppointmentController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public TestAppointmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int LDLAppID, int TestTypeID)
        {
            var LDLApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l => l.Id == LDLAppID, includeProperties: "Person,LicenseClass,ApplicationType,User");

            if (LDLApplication == null)
                return NotFound("تعذر تحميل بيانات طلب رخصة قيادة");

            TestAppointmentVM testAppointmentVM = new()
            {
                TestAppointments = (List<TestAppointment>)await _unitOfWork.TestAppointments.GetAllAsync(
                    filter: t => t.LocalDrivingLicenseApplicationId == LDLApplication.Id
                    && t.TestTypeId == TestTypeID),

                LocalDrivingLicenseApplication = LDLApplication,
                TestType = await _unitOfWork.TestTypes.GetAsync(t => t.Id == TestTypeID)
            };
            return View(testAppointmentVM);
        }

        public async Task<IActionResult> AddEdit(int? Id, int LDLAppId, int TestTypeId)
        {

            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (UserId == null)
                return NotFound("يجب تسجيل الدخول");

            var LDLApplicationFromDb = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(
                l => l.Id == LDLAppId, includeProperties: "Person,LicenseClass,User");

            var TestTypeFormDb = await _unitOfWork.TestTypes.GetAsync(t => t.Id == TestTypeId);

            if (LDLApplicationFromDb == null || TestTypeFormDb == null)
                return NotFound();

            TestAppointmentVM TestAppointmentVM = new()
            {
                LocalDrivingLicenseApplication = LDLApplicationFromDb,
                TestType = TestTypeFormDb,
            };

            var TestAppointmentFromDb = await _unitOfWork.TestAppointments.GetTestAppointmentForAppAsync(LDLAppId, TestTypeId);
            var RetakeTestApp = await _unitOfWork.ApplicationTypes.GetAsync(
                        a => a.Id == (int)Application.enApplicationType.RetakeTest);
            if (TestAppointmentFromDb != null)
            {
                TestAppointmentVM.HasPassedTest = await _unitOfWork.Tests.PersonPassTestAsync(TestAppointmentFromDb.Id);
                if (!TestAppointmentVM.HasPassedTest && TestAppointmentFromDb.IsLocked)
                {
                    TestAppointmentVM.RetakeTestApplicationFees = RetakeTestApp.Fees;
                }
            }

            if (Id == null || Id == 0)
            {
                // add mode

                if (TestAppointmentFromDb != null)
                {
                    if (_unitOfWork.TestAppointments.IsActiveAppointment(TestAppointmentFromDb))
                    {
                        TempData["ErrorMessage"] = " الشخص لديه موعد نشط, لا يمكن أضافة موعد اخر";
                        return RedirectToAction("Index", new { LDLAppID = LDLAppId, TestTypeID = TestTypeId });
                    }

                    if (TestAppointmentVM.HasPassedTest)
                    {
                        TempData["ErrorMessage"] = " الشخص قد أجتاز الاختبار بنجاح , لا يمكن أضافة موعد اخر";
                        return RedirectToAction("Index", new { LDLAppID = LDLAppId, TestTypeID = TestTypeId });
                    }
                }

                TestAppointmentVM.TestAppointment = new()
                {
                    PaidFees = TestTypeFormDb.Fees + TestAppointmentVM.RetakeTestApplicationFees,
                    IsLocked = false,
                    TestTypeId = TestTypeFormDb.Id,
                    LocalDrivingLicenseApplicationId = LDLApplicationFromDb.Id,
                    CreatedByUserId = UserId
                };

                return View(TestAppointmentVM);
            }
            else
            {
                // update mode
                TestAppointmentVM.TestAppointment = await _unitOfWork.TestAppointments.GetAsync(t => t.Id == Id);
                TestAppointmentVM.TestAppointment.PaidFees += TestAppointmentVM.RetakeTestApplicationFees;
                if (TestAppointmentVM.TestAppointment == null)
                    return NotFound("تعذر تحميل بيانات موعد الفحص");

                return View(TestAppointmentVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(TestAppointmentVM TestAppointmentVM)
        {
            if (ModelState.IsValid)
            {
                string Message = "";
                if (TestAppointmentVM.TestAppointment.Id == 0)
                {
                    // add mode
                    _unitOfWork.TestAppointments.UpdateRetakeTestApplicationOnFailure(TestAppointmentVM.TestAppointment, TestAppointmentVM.HasPassedTest);
                    await _unitOfWork.TestAppointments.AddAsync(TestAppointmentVM.TestAppointment);
                    Message = "تم أضافة الموعد بنجاح";
                }
                else
                {
                    _unitOfWork.TestAppointments.Update(TestAppointmentVM.TestAppointment);
                    Message = "تم تعديل الموعد بنجاح";
                }
                await _unitOfWork.SaveAsync();
                TempData["success"] = Message;
                return RedirectToAction("Index",
                    new
                    {
                        LDLAppID = TestAppointmentVM.TestAppointment.LocalDrivingLicenseApplicationId,
                        TestAppointmentVM.TestAppointment.TestTypeId
                    });
            }
            else
            {
                TestAppointmentVM testAppointmentVM = new()
                {
                    LocalDrivingLicenseApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l =>
                    l.Id == TestAppointmentVM.TestAppointment.LocalDrivingLicenseApplicationId),
                    TestType = await _unitOfWork.TestTypes.GetAsync(t => t.Id == TestAppointmentVM.TestAppointment.TestTypeId)
                };
            }
            return View(TestAppointmentVM);
        }

    }
}
