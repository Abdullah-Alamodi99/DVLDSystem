using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Models.ViewModels;
using DVLD.Utility;
using DVLD.Utility.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DVLDSystem.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DemoSettings _demoSettings;
        public UserController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IOptions<DemoSettings> demoOptions)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _demoSettings = demoOptions.Value;
        }
        public IActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public async Task<IActionResult> Card(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return NotFound();

            var User = await _unitOfWork.Users.GetAsync(u => u.Id == Id, includeProperties: "Person.Country");
            if (User == null)
                return NotFound("المستخدم غير موجود");

            return View(User);
        }

        public async Task<IActionResult> RoleManagement(string id)
        {
            var UserFromDb = await _unitOfWork.Users.GetAsync(u => u.Id == id, includeProperties: "Person");
            if (UserFromDb == null)
                return NotFound("تعذر تحميل بيانات المستخدم");

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                User = UserFromDb,
                RoleList = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
            };

            RoleVM.User.Role = (await _userManager.GetRolesAsync(UserFromDb)).FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> RoleManagement(RoleManagementVM RoleVM)
        {
            var user = await LoadUserWithPersonAsync(RoleVM.User.Id);
            if (user == null)
                return NotFound();

            var OldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var NewRole = RoleVM.User.Role;

            if (NewRole != OldRole)
            {
                // Prevent admin editing admin roles
                if (OldRole == SD.Role_Admin || NewRole == SD.Role_Admin)
                {
                    TempData["ErrorMessage"] = "لا يُسمح للمسؤولين بتغيير أدوارهم أو أدوار مسؤولين آخرين، حفاظًا على أمان النظام.";

                    RoleVM.User = user;
                    RoleVM.RoleList = GetRoles();
                    return View(RoleVM);
                }

                if (!string.IsNullOrEmpty(OldRole))
                    await _userManager.RemoveFromRoleAsync(user, OldRole);

                await _userManager.AddToRoleAsync(user, NewRole);
                TempData["success"] = "تم تغيير الدور بنجاح";
                return RedirectToAction("Index");
            }

            RoleVM.User = user;
            RoleVM.RoleList = GetRoles();
            return View(RoleVM);
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Users = await _unitOfWork.Users.GetAllAsync(includeProperties: "Person");
            return Json(new { data = Users });
        }

        [HttpGet]
        public async Task<IActionResult> IsUserExistForPersonID(int SelectedPersonId)
        {
            if (await _unitOfWork.Users.IsUserExistForPersonIDAsync(SelectedPersonId))
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }

        [HttpPut]
        public async Task<IActionResult> ActivateDeactivateUserAccount(string id)
        {
            var UserFromDb = await _unitOfWork.Users.GetAsync(u => u.Id == id, tracked: true);

            if (UserFromDb == null)
                return Json(new { success = false, message = "حدث خطأ! المستخدم غير موجود لا يمكن تنشيط/تجميد حساب المستخدم" });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ApplicationUser = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(ApplicationUser);
            if (roles.Contains(SD.Role_Admin))
            {
                return Json(new
                {
                    success = false,
                    message = "لا يمكن للمسؤولين تعطيل أو قفل حساباتهم الخاصة لضمان استمرارية إدارة النظام والحفاظ على الوصول إلى جميع الوظائف الإدارية."
                });
            }
            string Message = UserFromDb.IsActive ? "تم تجميد حساب المستخدم" : "تم تنشيط حساب المستخدم";
            _unitOfWork.Users.ActivateDeactivateUserAccount(UserFromDb);
            if (!await _unitOfWork.SaveAsync())
                return Json(new { success = false, message = "حدث خطأ لا يمكن تجميد الحساب" });
            return Json(new { success = true, message = Message });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var UserFromDb = await _unitOfWork.Users.GetAsync(u => u.Id == id);

            if (UserFromDb == null)
                return Json(new { success = false, message = "حدث خطأ! لا يمكن حذف المستخدم" });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationUser = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(applicationUser);
            if (roles.Contains(SD.Role_Admin))
            {
                return Json(new
                {
                    success = false,
                    message = "هذا المستخدم يمتلك صلاحيات إدارية ولا يمكن حذفه."
                });
            }

            _unitOfWork.Users.Remove(UserFromDb);
            if (await _unitOfWork.SaveAsync())
                return Json(new { success = true, message = " تم حذف المستخدم بنجاح" });

            return Json(new { success = false, message = "حدث خطأ! لا يمكن حذف المستخدم لوجود بيانات مرتبطة به" });
        }

        #endregion
        #region Private Methods

        public List<SelectListItem> GetRoles()
        {
            return _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();
        }
        private async Task<ApplicationUser?> LoadUserWithPersonAsync(string userId)
        {
            return await _unitOfWork.Users.GetAsync(u => u.Id == userId, includeProperties:"Person");
        }
        #endregion
    }
}
