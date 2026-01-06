using DVLD.Models;
using DVLD.Utility.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DVLDSystem.Areas.Identity.Pages.Account
{
    public class DemoLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DemoSettings _demoSettings;
        private readonly ILogger<DemoLoginModel> _logger;
        public DemoLoginModel(SignInManager<ApplicationUser> signInManager, IOptions<DemoSettings> demoOptions,
            ILogger<DemoLoginModel> logger)
        {
            _signInManager = signInManager;
            _demoSettings = demoOptions.Value;
            _logger = logger;

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!_demoSettings.Enabled)
            {
                _logger.LogWarning("Demo login attempted while demo features are disabled.");
                return Forbid();
            }

            var user = await _signInManager.UserManager.FindByNameAsync(_demoSettings.AdminUserName);

            if (user == null)
            {
                _logger.LogWarning("Demo user not found: {UserName}", _demoSettings.AdminUserName);
                return NotFound("«·„” Œœ„ «· Ã—Ì»Ì €Ì— „ÊÃÊœ.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                _demoSettings.AdminUserName,
                _demoSettings.AdminPassword,
                isPersistent: false,
                lockoutOnFailure:false);

            if(result.Succeeded)
            {
                _logger.LogInformation("Demo user {UserName} logged in successfully.", _demoSettings.AdminUserName);
                TempData["success"] = " „  ”ÃÌ· «·œŒÊ· »‰Ã«Õ (Ê÷⁄ «·⁄—÷ «· Ã—Ì»Ì)";
                return RedirectToAction("Index", "Home");
            }
            _logger.LogWarning("Demo login failed for user {UserName}", _demoSettings.AdminUserName);
            ModelState.AddModelError(string.Empty, "›‘·  ”ÃÌ· «·œŒÊ·.  Õﬁﬁ „‰ »Ì«‰«  «·œŒÊ· «· Ã—Ì»Ì….");
            return Page();
        }
    }
}
