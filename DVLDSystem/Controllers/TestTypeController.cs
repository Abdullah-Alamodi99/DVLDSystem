using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class TestTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TestTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var TestTypeList = await _unitOfWork.TestTypes.GetAllAsync();
            return View(TestTypeList);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return NotFound();

            var TestType = await _unitOfWork.TestTypes.GetAsync(x => x.Id == Id);

            if (TestType == null)
                return NotFound();

            return View(TestType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TestType TestType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.TestTypes.Update(TestType);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "تم تعديل الأختبار بنجاح";
                return RedirectToAction("Index");
            }
            return View(TestType);
        }
    }
}
