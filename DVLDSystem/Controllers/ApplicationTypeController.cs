using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLDSystem.Controllers
{
    [Authorize]
    public class ApplicationTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var ApplicationTypes = await _unitOfWork.ApplicationTypes.GetAllAsync();
            return View(ApplicationTypes);
        }

        public async Task <IActionResult> Edit(int? Id)
        {
            if (Id == null || Id == 0)
                return NotFound();
            var ApplicationType = await _unitOfWork.ApplicationTypes.GetAsync(x => x.Id == Id);
            if (ApplicationType == null)
                return NotFound();
            return View(ApplicationType);   
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationType ApplicationType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.ApplicationTypes.Update(ApplicationType);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "تم تعديل الطلب بنجاح";
                return RedirectToAction("Index");
            }
            return View(ApplicationType);
        }
    }
}
