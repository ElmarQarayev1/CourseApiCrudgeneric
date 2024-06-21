using System;
using Microsoft.AspNetCore.Http;

namespace Course.Service.Helpers
{
	public class FileManager
	{
        public static string Save(IFormFile f, string root, string folder)
        {
            string newFileName = Guid.NewGuid().ToString() + f.FileName;
            string path = Path.Combine(root, folder, newFileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                f.CopyTo(fileStream);
            }
            return newFileName;
        }
        public static bool Delete(string root, string folder, string fileName)
        {
            string path = Path.Combine(root, folder, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
