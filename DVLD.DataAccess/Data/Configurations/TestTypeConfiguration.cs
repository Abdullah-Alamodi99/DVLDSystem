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
    public class TestTypeConfiguration : IEntityTypeConfiguration<TestType>
    {
        public void Configure(EntityTypeBuilder<TestType> builder)
        {
            builder.Property(t => t.Fees).HasColumnType("smallmoney");
            builder.HasData(
                new TestType
                {
                    Id = 1,
                    Title = "فحص النظر",
                    Description = "يقيّم هذا الاختبار حدة إبصار المتقدم للتأكّد من أن لديه القدرة البصرية الكافية للقيادة بأمان",
                    Fees = 10
                },
                new TestType
                {
                    Id = 2,
                    Title = "اختبار تحريري نظري",
                    Description = "يقيم هذا الاختبار معرفة المتقدم بقواعد المرور، وعلامات الطريق، وأنظمة القيادة. يتكون عادةً من أسئلة متعددة الخيارات، ويجب على المتقدم اختيار الإجابة الصحيحة. يهدف الاختبار التحريري إلى التأكد من أن المتقدم يفهم قواعد الطريق ويمكنه تطبيقها في مختلف سيناريوهات القيادة",
                    Fees = 20
                },
                new TestType
                {
                    Id = 3,
                    Title = "الاختبار العملي (الشارع)",
                    Description = "يقيّم هذا الاختبار مهارات المتقدّم وقدراته على تشغيل مركبة آلية بأمان على الطرق العامة. يرافق المتقدم في السيارة فاحص مُرَخّص ويراقب أداءه في القيادة",
                    Fees = 35
                }
                );
        }
    }
}
