using GolbonWebRoad.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GolbonWebRoad.Infrastructure.Services
{
    public class VideoStorageService : IVideoStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VideoStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveVideoAsync(IFormFile file, string directoryPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("فایل ویدئو نامعتبر است.", nameof(file));
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalDirectoryPath = Path.Combine(wwwRootPath, directoryPath);

            if (!Directory.Exists(finalDirectoryPath))
            {
                Directory.CreateDirectory(finalDirectoryPath);
            }

            string fileName = "video_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(finalDirectoryPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/{directoryPath.Replace("\\", "/")}/{fileName}";
        }

        public async Task<List<string>> SaveVideosAsync(IEnumerable<IFormFile> files, string directoryPath)
        {
            var urls = new List<string>();
            if (files == null || !files.Any())
            {
                return urls;
            }

            foreach (var file in files)
            {
                var url = await SaveVideoAsync(file, directoryPath);
                urls.Add(url);
            }
            return urls;
        }
        public Task DeleteVideoAsync(string fileName, string directoryPath)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(directoryPath))
            {
                return Task.CompletedTask;
            }

            // ساخت مسیر کامل فایل فیزیکی در wwwroot
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, directoryPath, fileName);

            // بررسی وجود فایل و حذف آن
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
