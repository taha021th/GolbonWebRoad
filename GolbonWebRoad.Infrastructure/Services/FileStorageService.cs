using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GolbonWebRoad.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        //5 mb
        private const int MaxFileSize = 5*1024*1024;

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> SaveFileAsync(IFormFile file, string directoryPath)
        {
            ValidateImageFile(file);
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            string finalDirectoryPath = Path.Combine(wwwRootPath, directoryPath);

            if (!Directory.Exists(finalDirectoryPath))
                Directory.CreateDirectory(finalDirectoryPath);

            string fileName = $"{Guid.NewGuid()}.webp";
            string filePath = Path.Combine(finalDirectoryPath, fileName);

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                if (image.Width>1200)
                    image.Mutate(x => x.Resize(1200, 0));

                await image.SaveAsWebpAsync(filePath, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder { Quality=80 });

            }
            return $"/{directoryPath.Replace("\\", "/")}/{fileName}";
        }

        public async Task<List<string>> SaveFilesAsync(IEnumerable<IFormFile> files, string directoryPath)
        {
            var urls = new List<string>();
            if (files == null || !files.Any())
            {
                return urls;
            }

            foreach (var file in files)
            {
                var url = await SaveFileAsync(file, directoryPath);
                urls.Add(url);
            }
            return urls;
        }
        public Task DeleteFileAsync(string fileName, string directoryPath)
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
        private void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length==0)
            {
                throw new BadRequestException("هیچ فایلی برای آپلود انتخاب نشده است.");
            }
            if (file.Length > MaxFileSize)
            {
                throw new BadRequestException($"حجم فایل نمی تواند بیشتر از {MaxFileSize/1024/1024} مگابایت باشد.");
            }
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new BadRequestException("فرمت فایل نامعتبر است. فقط فایل های تصویری مجاز هستند.");
            }
        }
    }
}
