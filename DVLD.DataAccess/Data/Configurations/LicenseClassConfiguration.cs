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
    public class LicenseClassConfiguration : IEntityTypeConfiguration<LicenseClass>
    {
        public void Configure(EntityTypeBuilder<LicenseClass> builder)
        {
            builder.Property(t => t.Fees).HasColumnType("smallmoney");
            builder.HasData(
                new LicenseClass
                {
                    Id = 1,
                    Name = "الفئة 1 - دراجة نارية صغيرة",
                    Description = "يسمح للسائق بقيادة الدراجات النارية الصغيرة، وهو مناسب للدراجات النارية ذات السعة الصغيرة والقوة المحدودة.",
                    MinimumAllowedAge = 18,
                    DefualtValidityLength = 5,
                    Fees = 15
                },
                new LicenseClass
                {
                    Id = 2,
                    Name = "الفئة 2 - رخصة قيادة دراجات نارية ثقيلة",
                    Description = "رخصة قيادة الدراجات النارية الثقيلة (رخصة قيادة الدراجات النارية الكبيرة)",
                    MinimumAllowedAge = 21,
                    DefualtValidityLength = 5,
                    Fees = 30
                },
                new LicenseClass
                {
                    Id = 3,
                    Name = "الفئة 3 - رخصة القيادة العادية",
                    Description = "رخصة القيادة العادية (رخصة السيارة)",
                    MinimumAllowedAge = 18,
                    DefualtValidityLength = 10,
                    Fees = 20
                },
                new LicenseClass
                {
                    Id = 4,
                    Name = "الفئة 4 - تجاري",
                    Description = "رخصة القيادة التجارية (تاكسي/ليموزين)",
                    MinimumAllowedAge = 21,
                    DefualtValidityLength = 10,
                    Fees = 200
                },
                new LicenseClass
                {
                    Id = 5,
                    Name = "الفئة 5 - زراعي",
                    Description = "المركبات الزراعية ومركبات العمل المستخدمة في الزراعة أو البناء، مثل الجرارات وآلات الحراثة.",
                    MinimumAllowedAge = 21,
                    DefualtValidityLength = 10,
                    Fees = 50
                },
                new LicenseClass
                {
                    Id = 6,
                    Name = "الفئة 6 - الحافلات الصغيرة والمتوسطة",
                    Description = "رخصة الحافلات الصغيرة والمتوسطة.",
                    MinimumAllowedAge = 21,
                    DefualtValidityLength = 10,
                    Fees = 250
                },
                new LicenseClass
                {
                    Id = 7,
                    Name = "الفئة 7 - الشاحنات والمركبات الثقيلة",
                    Description = "رخصة الشاحنات والمركبات الثقيلة.",
                    MinimumAllowedAge = 21,
                    DefualtValidityLength = 10,
                    Fees = 300
                }
                );
        }
    }
}
