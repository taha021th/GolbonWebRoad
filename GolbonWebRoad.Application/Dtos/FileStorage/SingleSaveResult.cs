using GolbonWebRoad.Application.Enums;

namespace GolbonWebRoad.Application.Dtos.FileStorage
{
    // این آبجکت نتیجه برای آپلود تکی است
    public class SingleSaveResult
    {
        public FileValidationStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        // این دو فیلد فقط در صورت موفقیت (Success) پُر می‌شوند
        public string? Url { get; set; }
        public string? FileName { get; set; } // برای Rollback
    }
}
