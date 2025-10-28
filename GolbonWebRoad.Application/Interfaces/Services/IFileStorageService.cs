using GolbonWebRoad.Application.Dtos.FileStorage;
using Microsoft.AspNetCore.Http;

namespace GolbonWebRoad.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        // آپلود تکی: برای تصویر اصلی - در صورت خطا Exception پرتاب می‌کند
        Task<SingleSaveResult> SaveFileAsync(IFormFile file, string directoryPath);

        // آپلود دسته‌جمعی: برای گالری - خطاها را برمی‌گرداند
        Task<BatchSaveResult> SaveFilesAsync(IEnumerable<IFormFile> files, string directoryPath);

        // حذف فایل
        Task DeleteFileAsync(string fileName, string directoryPath);
    }
}