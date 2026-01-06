using DVLD.DataAccess.Data.Configurations;
using DVLD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLD.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Identity
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // Applications
        public DbSet<Application> Applications { get; set; }
        public DbSet<LocalDrivingLicenseApplication> LocalDrivingLicenseApplications { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }

        // Licenses
        public DbSet<License> Licenses { get; set; }
        public DbSet<LicenseClass> LicenseClasses { get; set; }
        public DbSet<DetainedLicense> DetainedLicenses { get; set; }
        public DbSet<InternationalLicense> InternationalLicenses { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        // Tests
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestAppointment> TestAppointments { get; set; }

        // People
        public DbSet<Person> People { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //disable cascade deletes
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }



            // Apply all configurations
            modelBuilder.ApplyAllConfigurations();

            modelBuilder.Entity<Application>().Property(t => t.PaidFees).HasColumnType("smallmoney");

            modelBuilder.Entity<Application>().ToTable("Applications");

            modelBuilder.Entity<LocalDrivingLicenseApplication>().ToTable("LocalDrivingLicenseApplications");

            modelBuilder.Entity<InternationalLicense>()
                .ToTable("InternationalLicenses");

            modelBuilder.Entity<TestAppointment>().Property(t => t.PaidFees).HasColumnType("smallmoney");
            modelBuilder.Entity<License>().Property(l => l.PaidFees).HasColumnType("smallmoney");

            modelBuilder.Entity<DetainedLicense>().Property(t => t.FineFees).HasColumnType("smallmoney");

            base.OnModelCreating(modelBuilder);
        }


    }
}
