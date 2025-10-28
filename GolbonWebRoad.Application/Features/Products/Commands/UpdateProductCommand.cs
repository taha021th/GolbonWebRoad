using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Enums;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest
    {

        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IFormFile? NewMainImage { get; set; } // تصویر اصلی جدید
        public List<IFormFile> NewImages { get; set; } = new();
        public List<string> ImagesToDelete { get; set; } = new();
        // Colors removed in favor of attribute-based variants
        // public List<ColorInputDto> NewColors { get; set; } = new();
        // public List<int> ColorsToDelete { get; set; } = new();

    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام محصول نمی تواند خالی باشد.")
                .MaximumLength(100).WithMessage("نام محصول نباید بیش از 100 کاراکتر باشد.");
            RuleFor(p => p.ShortDescription)
                .NotEmpty().WithMessage("توضیح کوتاه نمی تواند خالی باشد.")
                .MaximumLength(500).WithMessage("توضیح کوتاه نباید بیش از 500 کاراکتر باشد.");
            RuleFor(p => p.BasePrice)
                .GreaterThan(0).WithMessage("قیمت پایه باید بیشتر از 0 باشد.");
            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("انتخاب دسته‌بندی الزامی است.");
            RuleFor(p => p.BrandId)
                .GreaterThan(0).WithMessage("انتخاب برند الزامی است.")
                .When(p => p.BrandId.HasValue);
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private const string FileDirectory = "products"; // <--- ثابت برای مسیر

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateProductCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع فرآیند ویرایش محصول با شناسه {ProductId}", request.Id);

            var productToUpdate = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, joinImages: true);
            if (productToUpdate == null)
            {
                throw new NotFoundException($"محصول با شناسه {request.Id} یافت نشد.");
            }

            // --- فاز ۱: آماده‌سازی متغیرها برای تراکنش ---
            var uploadedFileNames = new List<string>(); // فایل‌های جدید آپلود شده، برای Rollback
            var filesToDeleteOnCommit = new List<string>(); // فایل‌های قدیمی، برای حذف *بعد* از موفقیت DB

            try
            {
                // --- فاز ۲: نگاشت و پردازش آپلودها (عملیات فایل‌سیستم) ---
                _mapper.Map(request, productToUpdate); // نگاشت پراپرتی‌های ساده

                // ۲-الف: پردازش تصویر اصلی جدید
                if (request.NewMainImage != null)
                {
                    // ۱. آپلود فایل جدید
                    var mainResult = await _fileStorageService.SaveFileAsync(request.NewMainImage, FileDirectory);

                    // <--- رفع باگ ۱: بررسی وضعیت نتیجه ---
                    if (mainResult.Status != FileValidationStatus.Success)
                    {
                        throw new ValidationException($"خطا در آپلود تصویر اصلی: {mainResult.ErrorMessage}");
                    }

                    // ۲. افزودن به لیست Rollback
                    uploadedFileNames.Add(mainResult.FileName);

                    // ۳. آماده‌سازی فایل قدیمی برای حذف (در صورت وجود)
                    if (!string.IsNullOrEmpty(productToUpdate.MainImageUrl))
                    {
                        // <--- رفع باگ ۲: حذف فایل به بعد از تراکنش موکول شد ---
                        filesToDeleteOnCommit.Add(Path.GetFileName(productToUpdate.MainImageUrl));
                    }

                    // ۴. به‌روزرسانی موجودیت (در حافظه)
                    productToUpdate.MainImageUrl = mainResult.Url;
                }

                // ۲-ب: پردازش تصاویر گالری جدید
                if (request.NewImages != null && request.NewImages.Any())
                {
                    foreach (var imageFile in request.NewImages)
                    {
                        var galleryResult = await _fileStorageService.SaveFileAsync(imageFile, FileDirectory);

                        // <--- رفع باگ ۱: بررسی وضعیت نتیجه ---
                        if (galleryResult.Status != FileValidationStatus.Success)
                        {
                            throw new ValidationException($"خطا در آپلود فایل گالری '{imageFile.FileName}': {galleryResult.ErrorMessage}");
                        }

                        uploadedFileNames.Add(galleryResult.FileName);
                        productToUpdate.Images.Add(new ProductImage
                        {
                            ImageUrl = galleryResult.Url,
                            IsMainImage = false // <--- رفع باگ ۳: همیشه false باشد
                        });
                    }
                }

                // --- فاز ۳: پردازش حذف‌ها (عملیات دیتابیس) ---
                if (request.ImagesToDelete != null && request.ImagesToDelete.Any())
                {
                    foreach (var imageUrl in request.ImagesToDelete)
                    {
                        var imageToRemove = productToUpdate.Images.FirstOrDefault(i => i.ImageUrl == imageUrl);
                        if (imageToRemove != null)
                        {
                            productToUpdate.Images.Remove(imageToRemove); // حذف از دیتابیس
                            // <--- رفع باگ ۲: حذف فایل به بعد از تراکنش موکول شد ---
                            filesToDeleteOnCommit.Add(Path.GetFileName(imageUrl)); // افزودن به لیست حذف از دیسک
                        }
                    }
                }

                // --- فاز ۴: ذخیره در دیتابیس (تراکنش اصلی) ---
                await _unitOfWork.CompleteAsync();

                // --- فاز ۵: حذف فایل‌های قدیمی (پس از موفقیت تراکنش) ---
                _logger.LogInformation("محصول {ProductId} با موفقیت در DB آپدیت شد. شروع حذف فایل‌های قدیمی.", request.Id);
                foreach (var fileName in filesToDeleteOnCommit)
                {
                    await DeleteFileSilentlyAsync(fileName, FileDirectory);
                }
            }
            catch (Exception ex)
            {
                // --- فاز Rollback ---
                _logger.LogCritical(ex, "خطا در آپدیت محصول {ProductId}. شروع Rollback فایل‌های آپلود شده.", request.Id);

                // <--- رفع باگ ۲: فقط فایل‌های *جدید* را حذف می‌کنیم ---
                foreach (var fileName in uploadedFileNames)
                {
                    await DeleteFileSilentlyAsync(fileName, FileDirectory);
                }

                throw; // پرتاب مجدد خطا
            }
        }

        /// <summary>
        /// متد کمکی برای حذف فایل در فاز Rollback یا Commit
        /// این متد خطاها را می‌گیرد تا خود فرآیند اصلی متوقف نشود
        /// </summary>
        private async Task DeleteFileSilentlyAsync(string fileName, string directoryPath)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(fileName, directoryPath);
                _logger.LogWarning("فایل {FileName} در مسیر {Directory} با موفقیت (silently) حذف شد.", fileName, directoryPath);
            }
            catch (Exception deleteEx)
            {
                // اگر حذف هم خطا داد، فقط لاگ می‌کنیم
                _logger.LogError(deleteEx, "خطا در هنگام حذف فایل {FileName} در مسیر {Directory}.", fileName, directoryPath);
            }
        }
    }
}