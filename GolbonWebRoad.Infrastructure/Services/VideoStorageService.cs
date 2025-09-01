using GolbonWebRoad.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GolbonWebRoad.Infrastructure.Services
{
    public class VideoStorageService : IVideoStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        // حداکثر حجم مجاز فایل به بایت (مثلاً 50 مگابایت)
        private const int MaxVideoSize = 50 * 1024 * 1024;
        // پسوندهای مجاز ویدئو
        private static readonly string[] AllowedExtensions = { ".mp4", ".mov", ".avi", ".mkv" };

        public VideoStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveVideoAsync(IFormFile file, string directoryPath)
        {
            // ۱. اعتبارسنجی کامل فایل ویدئویی
            ValidateVideoFile(file);

            // ایجاد مسیر نهایی در wwwroot
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalDirectoryPath = Path.Combine(wwwRootPath, directoryPath);
            if (!Directory.Exists(finalDirectoryPath))
            {
                Directory.CreateDirectory(finalDirectoryPath);
            }

            // ۲. ساخت نام فایل جدید و امن با حفظ پسوند اصلی
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string fileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(finalDirectoryPath, fileName);

            // ۳. ذخیره مستقیم فایل بدون پردازش
            // ما ویدئو را مستقیماً کپی می‌کنیم چون پردازش آن بسیار سنگین است.
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // ۴. برگرداندن URL عمومی
            return $"/{directoryPath.Replace("\\", "/")}/{fileName}";
        }

        // این متد بدون تغییر باقی می‌ماند
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

        // این متد بدون تغییر باقی می‌ماند
        public Task DeleteVideoAsync(string fileName, string directoryPath)
        {
            if (string.IsNullOrEmpty(fileName)) return Task.CompletedTask;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, directoryPath, Path.GetFileName(fileName));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        // متد خصوصی برای اعتبارسنجی فایل ویدئویی
        private void ValidateVideoFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("هیچ فایل ویدئویی برای آپلود انتخاب نشده است.");
            }

            if (file.Length > MaxVideoSize)
            {
                throw new ArgumentException($"حجم ویدئو نمی‌تواند بیشتر از {MaxVideoSize / 1024 / 1024} مگابایت باشد.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("فرمت ویدئو نامعتبر است. فقط فرمت‌های مجاز پشتیبانی می‌شوند.");
            }

            // بررسی اولیه نوع MIME
            if (!file.ContentType.StartsWith("video/"))
            {
                throw new ArgumentException("نوع فایل به عنوان ویدئو شناسایی نشد.");
            }
        }
    }
}