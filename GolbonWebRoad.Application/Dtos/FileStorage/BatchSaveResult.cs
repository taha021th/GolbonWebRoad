namespace GolbonWebRoad.Application.Dtos.FileStorage
{
    public class BatchSaveResult
    {
        // (آدرس URL, نام فایل برای Rollback)
        public List<(string Url, string FileName)> SuccessfulUploads { get; } = new();

        // لیست پیام‌های خطا برای فایل‌های ناموفق
        public List<string> FailedUploads { get; } = new();
    }
}
