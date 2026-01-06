using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.DataAccess.DemoSeedData
{
    public interface IDemoDataSeeder
    {
        Task SeedAsync();
    }
}
