using DVLD.DataAccess.Data;
using DVLD.Models;
using DVLD.Utility;
using DVLD.Utility.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DVLD.DataAccess.DemoSeedData
{
    public class DemoDataSeeder : IDemoDataSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DemoSettings _demoSettings;
        public DemoDataSeeder(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IOptions<DemoSettings> demoOptions)
        {
            _db = db;
            _userManager = userManager;
            _demoSettings = demoOptions.Value;
        }
        public async Task SeedAsync()
        {
            if (!_demoSettings.Enabled) return;

            // === Demo Admin ===
            if (await _userManager.FindByEmailAsync(_demoSettings.AdminEmail) == null)
            {
                var person = new Person
                {
                    NationalNo = "236876354",
                    FirstName = "محمد",
                    SecondName = "علي",
                    ThirdName = "محمد",
                    LastName = "ديمو",
                    DateOfBirth = new DateTime(1999, 1, 1),
                    Gender = 0,
                    Address = "المكلا",
                    Phone = "770000000",
                    Email = "moh@demo.com",
                    ImageUrl = null,
                    CountryId = 187
                };
                await _db.People.AddAsync(person);
                await _db.SaveChangesAsync();

                var DemoAdmin = new ApplicationUser
                {
                    UserName = _demoSettings.AdminUserName,
                    Email = _demoSettings.AdminEmail,
                    EmailConfirmed = true,
                    PersonId = person.Id
                };
                var result = await _userManager.CreateAsync(DemoAdmin, _demoSettings.AdminPassword);
                if (!result.Succeeded)
                {
                    _db.People.Remove(person);
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create demo admin: {errors}");
                }
                await _userManager.AddToRoleAsync(DemoAdmin, SD.Role_Admin);
            }

        }
    }
}
