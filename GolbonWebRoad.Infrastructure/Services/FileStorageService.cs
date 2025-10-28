using GolbonWebRoad.Application.Dtos.FileStorage;
using GolbonWebRoad.Application.Enums;
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
        private const int MaxFileSize = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // --- ۱. متد آپلود تکی ---
        public async Task<SingleSaveResult> SaveFileAsync(IFormFile file, string directoryPath)
        {
            var validationStatus = ValidateImageFileWithEnum(file, out string errorMessage);
            if (validationStatus != FileValidationStatus.Success)
            {
                return new SingleSaveResult
                {
                    Status = validationStatus,
                    ErrorMessage = errorMessage
                };
            }

            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string finalDirectoryPath = Path.Combine(wwwRootPath, directoryPath);

                if (!Directory.Exists(finalDirectoryPath))
                    Directory.CreateDirectory(finalDirectoryPath);

                // --- ۲. تغییر اصلی در اینجا ---
                // دیگر از Guid استفاده نمی‌کنیم، نام منحصربفرد می‌سازیم
                string fileName = GenerateUniqueFileName(file.FileName, finalDirectoryPath);
                // --- پایان تغییر ---

                string filePath = Path.Combine(finalDirectoryPath, fileName);

                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    if (image.Width > 1200)
                        image.Mutate(x => x.Resize(1200, 0));

                    await image.SaveAsWebpAsync(filePath, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder { Quality = 80 });
                }

                return new SingleSaveResult
                {
                    Status = FileValidationStatus.Success,
                    Url = $"/{directoryPath.Replace("\\", "/")}/{fileName}",
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return new SingleSaveResult
                {
                    Status = FileValidationStatus.InvalidExtension,
                    ErrorMessage = $"خطا در پردازش فایل '{file.FileName}': {ex.Message}"
                };
            }
        }

        // --- ۲. متد آپلود دسته‌جمعی (برای گالری) ---
        public async Task<BatchSaveResult> SaveFilesAsync(IEnumerable<IFormFile> files, string directoryPath)
        {
            var result = new BatchSaveResult();
            if (files == null || !files.Any())
            {
                return result;
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalDirectoryPath = Path.Combine(wwwRootPath, directoryPath);
            if (!Directory.Exists(finalDirectoryPath))
                Directory.CreateDirectory(finalDirectoryPath);

            foreach (var file in files)
            {
                var validationStatus = ValidateImageFileWithEnum(file, out string errorMessage);
                if (validationStatus != FileValidationStatus.Success)
                {
                    result.FailedUploads.Add(errorMessage);
                    continue;
                }

                try
                {
                    // --- ۳. تغییر اصلی در اینجا (مشابه متد تکی) ---
                    string fileName = GenerateUniqueFileName(file.FileName, finalDirectoryPath);
                    // --- پایان تغییر ---

                    string filePath = Path.Combine(finalDirectoryPath, fileName);

                    using (var image = await Image.LoadAsync(file.OpenReadStream()))
                    {
                        if (image.Width > 1200)
                            image.Mutate(x => x.Resize(1200, 0));

                        await image.SaveAsWebpAsync(filePath, new SixLabors.ImageSharp.Formats.Webp.WebpEncoder { Quality = 80 });
                    }

                    var url = $"/{directoryPath.Replace("\\", "/")}/{fileName}";
                    result.SuccessfulUploads.Add((url, fileName));
                }
                catch (Exception ex)
                {
                    result.FailedUploads.Add($"خطا در پردازش فایل '{file.FileName}': {ex.Message}");
                }
            }
            return result;
        }

        // --- متد حذف (بدون تغییر) ---
        public Task DeleteFileAsync(string fileName, string directoryPath)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(directoryPath))
                return Task.CompletedTask;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, directoryPath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }

        // --- متد ولیدیشن (بدون تغییر) ---
        private FileValidationStatus ValidateImageFileWithEnum(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (file == null || file.Length == 0)
            {
                errorMessage = "هیچ فایلی برای آپلود انتخاب نشده است.";
                return FileValidationStatus.FileNullOrEmpty;
            }
            if (file.Length > MaxFileSize)
            {
                errorMessage = $"فایل '{file.FileName}' به دلیل حجم بالا ({Math.Round(file.Length / 1024.0 / 1024.0, 2)} مگابایت) نادیده گرفته شد. (حداکثر {MaxFileSize / 1024 / 1024} مگابایت)";
                return FileValidationStatus.FileTooLarge;
            }
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                errorMessage = $"فایل '{file.FileName}' به دلیل فرمت نامعتبر ({extension}) نادیده گرفته شد.";
                return FileValidationStatus.InvalidExtension;
            }
            return FileValidationStatus.Success;
        }

        // --- ۴. متد کمکی جدید برای تولید نام منحصربفرد ---
        private string GenerateUniqueFileName(string originalFileName, string directoryPath)
        {
            // نام فایل بدون پسوند را استخراج می‌کند
            string baseName = Path.GetFileNameWithoutExtension(originalFileName);

            // کاراکترهای غیرمجاز را با آندرلاین جایگزین می‌کند
            string sanitizedName = Path.GetInvalidFileNameChars()
                .Aggregate(baseName, (current, c) => current.Replace(c.ToString(), "_"));

            // پسوند .webp را اضافه می‌کند
            string finalName = $"{sanitizedName}.webp";
            string fullPath = Path.Combine(directoryPath, finalName);

            // اگر فایلی با این نام وجود داشت، یک عدد به انتهای آن اضافه می‌کند
            int counter = 1;
            while (File.Exists(fullPath))
            {
                finalName = $"{sanitizedName} ({counter}).webp";
                fullPath = Path.Combine(directoryPath, finalName);
                counter++;
            }

            return finalName;
        }
    }
}