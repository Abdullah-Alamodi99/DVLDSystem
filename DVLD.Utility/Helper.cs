using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD.Utility
{
    public class Helper
    {
        private static string GenerateUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        private static string GetFileNameExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }

        public static string UploadFile(IFormFile file, string path)
        {
            if (file != null)
            {
                string fileName = file.FileName;
                if (GetFileNameExtension(fileName).ToLower() != ".pdf")
                {
                    fileName = GenerateUniqueKey() + GetFileNameExtension(file.FileName);
                }

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return fileName;
            }
            return string.Empty;
        }

        public static bool DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
