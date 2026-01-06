using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PersonController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddEdit(int? Id)
        {
            var CountriesList = await _unitOfWork.Countries.GetAllAsync();
            ViewData["Countries"] = CountriesList.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });

            if (Id == null || Id == 0)
            {
                // adding new person
                return View(new Person());
            }
            else
            {
                // update person
                var PersonFromDb = await _unitOfWork.People.GetAsync(p => p.Id == Id);
                if (PersonFromDb == null)
                    return NotFound();
                return View(PersonFromDb);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Person Person, IFormFile? File)
        {
            Person PersonFromDb = await _unitOfWork.People.GetAsync(p => p.Id == Person.Id, tracked: false);

            // add mode
            if (Person.Id == 0)
            {
                if (await _unitOfWork.People.IsNationalNoExistAsync(Person.NationalNo))
                {
                    ModelState.AddModelError("Person.NationalNo", "الرقم الوطني موجود");
                }
            }

            else
            {
                // update mode
                if (PersonFromDb == null)
                    return NotFound();

                //NationalNo has been changed
                if (PersonFromDb.NationalNo != Person.NationalNo)
                {
                    if (await _unitOfWork.People.IsNationalNoExistAsync(Person.NationalNo))
                    {
                        ModelState.AddModelError("Person.NationalNo", "الرقم الوطني موجود");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                string Message = string.Empty;
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (File != null)
                {
                    string PersonImgPath = "images/person";
                    string FullPath = Path.Combine(wwwRootPath, PersonImgPath);
                    Person.ImageUrl = Helper.UploadFile(File, FullPath);

                    // delete old image
                    if (PersonFromDb != null)
                    {
                        if (!string.IsNullOrEmpty(PersonFromDb.ImageUrl))
                        {
                            string OldImgPath = Path.Combine(FullPath, PersonFromDb.ImageUrl);
                            Helper.DeleteFile(OldImgPath);
                        }
                    }


                }
                if (Person.Id == 0)
                {
                    // adding new person
                    await _unitOfWork.People.AddAsync(Person);
                    Message = "تم أضافة الشخص بنجاح";
                }
                else
                {
                    // update person

                    _unitOfWork.People.Update(Person);
                    Message = "تم تعديل بيانات الشخص بنجاح";
                }
                await _unitOfWork.SaveAsync();
                TempData["success"] = Message;
                return RedirectToAction("Index");
            }
            else
            {
                var CountriesList = await _unitOfWork.Countries.GetAllAsync();
                ViewData["Countries"] = CountriesList.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
            }
            return View(Person);
        }

        public async Task<IActionResult> DeleteImage(int PersonId)
        {
            var PersonFromDb = await _unitOfWork.People.GetAsync(p => p.Id == PersonId);
            if (PersonFromDb != null)
            {
                if (!string.IsNullOrEmpty(PersonFromDb.ImageUrl))
                {
                    string OldImgPath = @"images/person/" + PersonFromDb.ImageUrl;
                    Helper.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath, OldImgPath));
                    PersonFromDb.ImageUrl = null;
                    _unitOfWork.People.Update(PersonFromDb);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "تم حذف الصورة الشخصية بنجاح";
                    return RedirectToAction(nameof(AddEdit), new { Id = PersonFromDb.Id });
                }
            }
            return RedirectToAction(nameof(AddEdit), new { Id = PersonId });
        }

        public async Task<IActionResult> Card(int Id)
        {
            var Person = await _unitOfWork.People.GetAsync(p => p.Id == Id, includeProperties: "Country");
            if (Person == null)
                return NotFound();

            return View(Person);
        }

        public IActionResult EmptyPersonCard()
        {
            return PartialView("_EmptyPersonCard");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _unitOfWork.People.GetAllAsync(includeProperties: "Country");

            return Json(new { data = people });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var PersonToBeDeleted = await _unitOfWork.People.GetAsync(p => p.Id == id);
            string PersonImg = "";
            if (PersonToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(PersonToBeDeleted.ImageUrl))
                    PersonImg = $"{PersonToBeDeleted.ImageUrl}";

                _unitOfWork.People.Remove(PersonToBeDeleted);
                if (await _unitOfWork.SaveAsync())
                {
                    // delete person image
                    if (!string.IsNullOrEmpty(PersonImg))
                    {
                        string PersonImgToBelDeletedPath = "images/person/" + PersonImg;
                        Helper.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath, PersonImgToBelDeletedPath));
                    }
                    return Json(new { success = true, message = "تم حذف الشخص بنجاح" });
                }
                return Json(new { success = false, message = "حدث خطأ : لا يمكن حذف هذا الشخص لوجود بيانات مرتبطة به" });
            }
            return Json(new { success = false, message = "حدث خطأ : الشخص غير موجود" });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string GetBy, string SearchValue)
        {
            Person? Person = null;

            switch (GetBy)
            {
                case "Id":
                    if (int.TryParse(SearchValue, out int PersonId))
                        Person = await _unitOfWork.People.GetAsync(p => p.Id == PersonId, includeProperties: "Country");
                    break;
                case "NationalNo":
                    Person = await _unitOfWork.People.GetAsync(p => p.NationalNo == SearchValue, includeProperties: "Country");
                    break;
                default:
                    break;
            }
            if (Person == null)
                return NotFound();

            Person.IsPersonExitForUser = await _unitOfWork.Users.IsUserExistForPersonIDAsync(Person.Id);

            return PartialView("_PersonCard", Person);
        }
        #endregion
    }
}
