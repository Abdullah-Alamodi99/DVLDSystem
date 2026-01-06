using DVLD.DataAccess.Data;
using DVLD.DataAccess.DbInitializer;
using DVLD.DataAccess.Repository.IRepository;
using DVLDSystem.Custom;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using DVLD.DataAccess.DemoSeedData;
using DVLD.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using DVLD.Utility;
using DVLD.Models;
using DVLD.Utility.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("ar")
        };
    options.DefaultRequestCulture = new RequestCulture(culture:"ar", uiCulture:"ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SeedAdminSettings>(builder.Configuration.GetSection("AdminSeed"));
builder.Services.Configure<DemoSettings>(builder.Configuration.GetSection("DemoSeed"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders().AddErrorDescriber<CustomIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options => {
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// register demo settings
builder.Services.Configure<DemoSettings>(builder.Configuration.GetSection("Demo"));

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IDemoDataSeeder, DemoDataSeeder>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    try
    {
        await dbInitializer.Initialize();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }

    var demoSeedSettings = scope.ServiceProvider
        .GetRequiredService<IOptions<DemoSettings>>().Value;

    if (demoSeedSettings.Enabled)
    {
        var demoSeeder = scope.ServiceProvider.GetRequiredService<IDemoDataSeeder>();
        await demoSeeder.SeedAsync();
    }
}

app.Run();
