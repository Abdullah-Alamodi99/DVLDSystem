using DVLD.DataAccess.Repository;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DVLDSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using DVLD.Utility;

namespace DVLDSystem.Controllers
{
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_LicenseOfficer}")]
    public class LicenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public LicenseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(IUnitOfWork unitOfWork)
        {
            return View();
        }

        public async Task<IActionResult> Issue(int? LDLApplicationId)
        {
            var LicenseVM = await _FillApplicationInfoOnLoadAsync(LDLApplicationId);
            if (LicenseVM == null)
                return NotFound("تعذر تحميل بيانات طلب رخصة القيادة");

            return View(LicenseVM);
        }
        [HttpPost]
        public async Task<IActionResult> Issue(LicenseVM LicenseVM)
        {
            if (ModelState.IsValid)
            {

                bool LicenseIssued = await _unitOfWork.Licenses.IssueLicenseFirstTimeAsync(LicenseVM);
                if (LicenseIssued)
                {
                    TempData["success"] = "تم إصدار الرخصة بنجاح";
                    return RedirectToAction("Index", "LocalDrivingLicenseApplication");
                }
                else
                {
                    TempData["ErrorMessage"] = "حدث خطأ! تعذر إصدار الرخصة";
                }
            }
            else
            {
                var LicenseVMData = await _FillApplicationInfoOnLoadAsync(LicenseVM.License.ApplicationId);
                if (LicenseVMData == null)
                    return NotFound("تعذر تحميل بيانات طلب رخصة القيادة");
                LicenseVM = LicenseVMData;
            }

            return View(LicenseVM);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LicenseInfoByApplicationId(int? ApplicationId)
        {
            if (ApplicationId == null || ApplicationId == 0)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الرخصة");

            var License = await _unitOfWork.Licenses.GetLicenseByApplicationIdAsync(ApplicationId);

            if (License == null)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الرخصة");

            return View(License);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LicenseInfoById(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الرخصة");

            var License = await _unitOfWork.Licenses.GetLicenseByIdAsync(Id);

            if (License == null)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الرخصة");

            return View("LicenseInfoByApplicationId", License);
        }

        [AllowAnonymous]
        public async Task<IActionResult> PersonLicenseHistoryByLicenseId(int? Id, int? PersonId)
        {
            if (Id == null || Id == 0)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الرخصة");
            if (PersonId == null || PersonId == 0)
                return NotFound("حدث خطأ! تعذر تحميل بيانات الشخص");

            LicenseVM LicenseVM = new()
            {
                Licenses = await _unitOfWork.Licenses.GetAllAsync(l => l.Id == Id,
                includeProperties: "LicenseClass,Application"),
                InternationalLicenses = await _unitOfWork.InternationalLicenses.GetAllAsync(filter: il => il.IssuedUsingLocalLicenseId == Id),
                Person = await _unitOfWork.People.GetAsync(p => p.Id == PersonId, includeProperties: "Country")
            };

            return View(LicenseVM);

        }

        public async Task<IActionResult> RenewLicense()
        {
            LicenseOperationVM LicenseOperationVM = await _FillLicenseInfoOnLoadAsync();
            if (LicenseOperationVM.NewLicense.CreatedByUserId == null)
                return Unauthorized("يجب تسجيل الدخول");
            ViewBag.CanIssueForOperation = false;
            return View(LicenseOperationVM);
        }

        [HttpPost]
        public async Task<IActionResult> RenewLicense(LicenseOperationVM LicenseOperationVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "تعذر تجديد الرخصة, بيانات الادخال غير صحيحة";
                return View(LicenseOperationVM);
            }
            var NewLicense = await _unitOfWork.Licenses.RenewLicenseAsync(LicenseOperationVM.NewLicense, LicenseOperationVM.OldLicense);
            if (NewLicense != null)
            {
                TempData["success"] = "تم تجديد الرخصة بنجاح";
                return RedirectToAction("LicenseInfoById", new { Id = NewLicense.Id });
            }

            ViewBag.CanIssueForOperation = false;
            TempData["ErrorMessage"] = "تعذر تجديد الرخصة";
            return View(LicenseOperationVM);
        }
        public async Task<IActionResult> ReplacementLicense()
        {
            LicenseOperationVM LicenseOperationVM = await _FillLicenseInfoOnLoadAsync();
            if (LicenseOperationVM.NewLicense.CreatedByUserId == null)
                return Unauthorized("يجب تسجيل الدخول");
            ViewBag.CanIssueForOperation = false;
            return View(LicenseOperationVM);
        }

        [HttpPost]
        public async Task<IActionResult> ReplacementLicense(LicenseOperationVM LicenseOperationVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "تعذر تجديد الرخصة, بيانات الادخال غير صحيحة";
                return View(LicenseOperationVM);
            }
            var NewLicense = await _unitOfWork.Licenses.ReplaceLicenseAsync(LicenseOperationVM.NewLicense, LicenseOperationVM.OldLicense);
            if (NewLicense != null)
            {
                TempData["success"] = "تم إستبدال الرخصة بنجاح";
                return RedirectToAction("LicenseInfoById", new { Id = NewLicense.Id });
            }

            ViewBag.CanIssueForOperation = false;
            TempData["ErrorMessage"] = "تعذر تجديد الرخصة";
            return View(LicenseOperationVM);
        }

        public IActionResult EmptyLocalLicenseCard()
        {
            return PartialView("_EmptyLocalLicenseCard");
        }

        #region NORMAL FUNCTIONS
        private async Task<LicenseVM>? _FillApplicationInfoOnLoadAsync(int? LDLApplicationId)
        {
            if (LDLApplicationId == 0 || LDLApplicationId == null)
                return null;

            var LDLApplication = await _unitOfWork.LocalDrivingLicenseApplications.GetAsync(l => l.Id == LDLApplicationId,
                includeProperties: "Person,LicenseClass,ApplicationType,User");

            if (LDLApplication == null)
                return null;

            var LicenseClass = await _unitOfWork.LicenseClasses.GetAsync(l => l.Id == LDLApplication.LicenseClassId);
            LicenseVM LicenseVM = new()
            {
                LocalDrivingLicenseApplication = LDLApplication,
                License = new()
                {
                    Id = 0,
                    IssueDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddYears(LicenseClass.DefualtValidityLength),
                    PaidFees = LicenseClass.Fees,
                    IsActive = true,
                    IssueReason = (byte)License.enIssueReason.NewLicense,
                    ApplicationId = LDLApplication.Id,
                    LicenseClassId = LDLApplication.LicenseClassId,
                    CreatedByUserId = LDLApplication.User.Id
                }
            };
            return LicenseVM;
        }
        private async Task<LicenseOperationVM> _FillLicenseInfoOnLoadAsync()
        {
            var CurrentUser = await _unitOfWork.Users.GetAsync(u => u.Id == User.GetCurrentUserId());
            LicenseOperationVM LicenseOperationVM = new()
            {
                NewLicense = new()
                {
                    IssueDate = DateTime.Now,
                    CreatedByUserId = CurrentUser.Id,
                    IsActive = true,
                    User = CurrentUser,
                    Application = new()
                    {
                        ApplicationDate = DateTime.Now,
                        LastStatusDate = DateTime.Now,
                        CreatedByUserId = CurrentUser.Id,
                        ApplicationStatus = (byte)Application.enStatus.New,
                    }
                }
            };
            return LicenseOperationVM;
        }

        private List<string> _ValidateOldLicense(License OldLicense, LicenseOperationVM.enLicenseOperationType ValidateFor)
        {
            List<string> Errors = new();
            if (ValidateFor == LicenseOperationVM.enLicenseOperationType.Renew)
            {
                if (OldLicense == null)
                {
                    Errors.Add("الرخصة غير موجودة");
                    return Errors;
                }
                if (!OldLicense.IsActive)
                {
                    Errors.Add("الرخصة غير نشطة, لا يمكن التجديد");
                    return Errors;
                }
                if (OldLicense.IsDetained)
                {
                    Errors.Add("الرخصة محجوزة , لا يمكن التجديد");
                    return Errors;
                }
                if (!_unitOfWork.Licenses.IsExpired(OldLicense))
                {
                    Errors.Add("الرخصة غير منتهية الصلاحية , لا يمكن التجديد");
                    return Errors;
                }
            }
            else
            {
                if (!OldLicense.IsActive)
                {
                    Errors.Add("الرخصة غير نشطة, لا يمكن اصدار بدل فاقد او تالف");
                    return Errors;
                }
            }
            return Errors;
        }
        #endregion


        #region API CALLS
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetLicense(int LocalLicenseId)
        {
            var License = await _unitOfWork.Licenses.GetLicenseByIdAsync(LocalLicenseId);
            if (License == null)
                return NotFound();

            return PartialView("_LicenseInfo", License);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SearchLocalLicense(int OldLicenseId, LicenseOperationVM.enLicenseOperationType OperationType)
        {
            var OldLicense = await _unitOfWork.Licenses.GetAsync(l => l.Id == OldLicenseId, includeProperties: "LicenseClass,Application.Person");
            LicenseOperationVM LicenseOperationVM = await _FillLicenseInfoOnLoadAsync();
            var RenewDrivingLicenseApp = await _unitOfWork.ApplicationTypes.GetAsync(a => a.Id == (int)Application.enApplicationType.RenewDrivingLicense);
            var ReplaceDamagedDrivingLicenseApp = await _unitOfWork.ApplicationTypes.GetAsync(a => a.Id == (int)Application.enApplicationType.ReplaceDamagedDrivingLicense);
            var ReplaceLostDrivingLicenseApp = await _unitOfWork.ApplicationTypes.GetAsync(a => a.Id == (int)Application.enApplicationType.ReplaceLostDrivingLicense);
            switch (OperationType)
            {
                case LicenseOperationVM.enLicenseOperationType.Renew:
                    LicenseOperationVM.NewLicense.IssueReason = (byte)License.enIssueReason.RenewLicense;
                    LicenseOperationVM.NewLicense.Application.PaidFees = RenewDrivingLicenseApp.Fees;
                    LicenseOperationVM.NewLicense.Application.ApplicationTypeId = (int)Application.enApplicationType.RenewDrivingLicense;
                    break;
                case LicenseOperationVM.enLicenseOperationType.DamagedReplacement:
                    LicenseOperationVM.NewLicense.IssueReason = (byte)License.enIssueReason.ReplaceForDamage;
                    LicenseOperationVM.NewLicense.Application.PaidFees = ReplaceDamagedDrivingLicenseApp.Fees;
                    LicenseOperationVM.NewLicense.Application.ApplicationTypeId = (int)Application.enApplicationType.ReplaceDamagedDrivingLicense;
                    break;
                case LicenseOperationVM.enLicenseOperationType.LostReplacement:
                    LicenseOperationVM.NewLicense.IssueReason = (byte)License.enIssueReason.ReplaceForLost;
                    LicenseOperationVM.NewLicense.Application.PaidFees = ReplaceLostDrivingLicenseApp.Fees;
                    LicenseOperationVM.NewLicense.Application.ApplicationTypeId = (int)Application.enApplicationType.ReplaceLostDrivingLicense;
                    break;
            }
            if (OldLicense != null)
            {
                LicenseOperationVM.OldLicense = OldLicense;
                LicenseOperationVM.NewLicense.PaidFees = OldLicense.LicenseClass.Fees;
                LicenseOperationVM.NewLicense.LicenseClassId = OldLicense.LicenseClassId;
                LicenseOperationVM.NewLicense.DriverId = OldLicense.DriverId;
                LicenseOperationVM.NewLicense.Application.PersonId = _unitOfWork.Drivers.GetPersonIdByDriverId(OldLicense.DriverId);
                LicenseOperationVM.NewLicense.ExpirationDate = DateTime.Now.AddYears(OldLicense.LicenseClass.DefualtValidityLength);

                var ValidationErrors = _ValidateOldLicense(LicenseOperationVM.OldLicense, OperationType);
                if (ValidationErrors.Any())
                {
                    TempData["ErrorMessage"] = ValidationErrors.First();
                    ViewBag.CanIssueForOperation = false;
                }
                else
                    ViewBag.CanIssueForOperation = true;
            }
            else
            {
                ViewBag.CanIssueForOperation = false;
                TempData["ErrorMessage"] = "الرخصة غير موجودة";
            }
            if (OperationType == LicenseOperationVM.enLicenseOperationType.Renew)
                return View("RenewLicense", LicenseOperationVM);

            return View("ReplacementLicense", LicenseOperationVM);

        }

        #endregion
    }
}
