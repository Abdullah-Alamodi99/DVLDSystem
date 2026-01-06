using DVLD.DataAccess.Data;
using DVLD.DataAccess.Repository.IRepository;
using DVLD.Models;
using DVLD.Utility;
using DVLD.Utility.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVLD.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IDbInitializer> _logger;
        private readonly SeedAdminSettings _settings;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IDbInitializer> logger,
            IOptions<SeedAdminSettings> options,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _db = db;
        }

        public async Task Initialize()
        {
            // Apply pending migrations if needed
            try
            {
                if ((await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Migration error: {ex.Message}");
            }

            await SeedCountriesAsync();

            // Create roles if not exist
            if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_LicenseOfficer));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Examiner));
            }

            // Check if admin already exists
            const string AdminEmail = "admin@abdullah.com";
            var existingAdmin = await _userManager.FindByEmailAsync(AdminEmail);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists, skipping creation.");
                return;
            }

            // Use a transaction for atomic creation of Person + ApplicationUser
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var CountryId = await GetAdminCountryIdAsync();

                var person = new Person
                {
                    NationalNo = "123983645",
                    FirstName = "عبدالله",
                    SecondName = "محمد",
                    ThirdName = "عبدالله",
                    LastName = "العمودي",
                    Gender = 0,
                    DateOfBirth = new DateTime(1999, 2, 1),
                    Address = "حضرموت - المكلا",
                    Phone = "710000000",
                    Email = "abdullah00@gmail.com",
                    ImageUrl = null,
                    CountryId = CountryId
                };

                await _db.People.AddAsync(person);
                await _db.SaveChangesAsync();

                // Create the ApplicationUser linked to the Person
                var adminUserName = _settings.UserName;
                var adminPassword = _settings.Password;

                if (string.IsNullOrEmpty(adminUserName) || string.IsNullOrEmpty(adminPassword))
                {
                    _logger.LogError("Admin username or password is not configured properly.");
                    await transaction.RollbackAsync();
                    return;
                }

                var user = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = AdminEmail,
                    PersonId = person.Id,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, adminPassword);

                if (!result.Succeeded)
                {
                    _db.People.Remove(person);
                    await _db.SaveChangesAsync();

                    _logger.LogError("Failed to create default admin user: " +
                                     string.Join(", ", result.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync();
                    return;
                }

                // Assign admin role
                await _userManager.AddToRoleAsync(user, SD.Role_Admin);

                // Commit transaction
                await transaction.CommitAsync();
                _logger.LogInformation("Admin user created and assigned to Admin role.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error during seeding admin user: {ex.Message}");
            }
        }

        private async Task SeedCountriesAsync()
        {
            if (!_db.Countries.Any())
            {
                try
                {
                    // Use AppContext.BaseDirectory to make path safe for publish
                    var path = Path.Combine(AppContext.BaseDirectory, "Data", "countries.json");
                    if (!File.Exists(path))
                    {
                        _logger.LogError($"Countries JSON file not found at: {path}");
                        return;
                    }

                    var countriesJson = await File.ReadAllTextAsync(path);
                    var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

                    if (countries != null && countries.Any())
                    {
                        await _db.Countries.AddRangeAsync(countries);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation("Seeded countries successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("countries.json is empty or could not be deserialized.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error seeding countries: {ex.Message}");
                }
            }
        }

        private async Task<int> GetAdminCountryIdAsync()
        {
            string CountryName = "اليمن";
            var country = await _db.Countries.SingleOrDefaultAsync(c => c.Name == CountryName);
            if(country == null)
            {
                country = new Country { Name = CountryName };
                await _db.Countries.AddAsync(country);
                await _db.SaveChangesAsync();
            }
            return country.Id;
        }
    }
}
