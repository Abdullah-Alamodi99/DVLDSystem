using DVLD.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.Data.Configurations
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
    {
        public void Configure(EntityTypeBuilder<ApplicationType> builder)
        {
            builder.Property(t => t.Fees).HasColumnType("smallmoney");
            builder.HasData(
                new ApplicationType
                {
                    Id = 1,
                    Title = "خدمة رخصة القيادة المحلية الجديدة",
                    Fees = 15
                },
                new ApplicationType
                {
                    Id = 2,
                    Title = "خدمة تجديد الرخصة",
                    Fees = 7
                },
                new ApplicationType
                {
                    Id = 3,
                    Title = "بدل رخصة قيادة مفقودة",
                    Fees = 10
                },
                new ApplicationType
                {
                    Id = 4,
                    Title = "بدل رخصة قيادة تالفة",
                    Fees = 5
                },
                new ApplicationType
                {
                    Id = 5,
                    Title = "فك رخصة قيادة محجوزة",
                    Fees = 15
                },
                new ApplicationType
                {
                    Id = 6,
                    Title = "رخصة دولية جديدة",
                    Fees = 51
                },
                new ApplicationType
                {
                    Id = 7,
                    Title = "إعادة الاختبار",
                    Fees = 5
                }
                );
        }
    }
}
