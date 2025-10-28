using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Enums;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<int>
    {
        public string? Slog { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IFormFile? MainImage { get; set; } // تصویر اصلی محصول
        // Colors removed in favor of attribute-based variants
        // public List<ColorInputDto> Colors { get; set; } = new();
        public List<IFormFile> Images { get; set; } = new();
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام محصول نمی تواند خالی باشد.")
                .MaximumLength(100).WithMessage("نام محصول نمی تواند بیشتر از 100 کاراکتر باشد.");
            RuleFor(p => p.ShortDescription)
                .NotEmpty().WithMessage("توضیح کوتاه نمی تواند خالی باشد.")
                .MaximumLength(500).WithMessage("توضیح کوتاه نباید بیش از 500 کاراکتر باشد.");
            RuleFor(p => p.BasePrice)
                .GreaterThan(0).WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("انتخاب دسته‌بندی الزامی است.");
            RuleFor(p => p.BrandId)
                .GreaterThan(0).WithMessage("انتخاب برند الزامی است.")
                .When(p => p.BrandId.HasValue);
        }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<CreateProductCommandHandler> _logger; // ۲. ILogger را تعریف کنید
        private const string FileDirectory = "products";

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService=fileStorageService;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع فرآیند ایجاد محصول '{ProductName}'.", request.Name);
            var product = _mapper.Map<Product>(request);
            var uploadedFileNames = new List<string>();

            try
            {
                // --- فاز اول: آپلود تصویر اصلی ---
                if (request.MainImage != null)
                {
                    // <--- ۳. نتیجه را در متغیر SingleSaveResult دریافت کنید
                    var mainImageResult = await _fileStorageService.SaveFileAsync(request.MainImage, FileDirectory);

                    // <--- ۴. وضعیت (Status) را چک کنید
                    if (mainImageResult.Status != FileValidationStatus.Success)
                    {
                        // <--- ۵. در صورت خطا، یک Exception پرتاب کنید تا catch اجرا شود
                        throw new ValidationException(mainImageResult.ErrorMessage);
                    }

                    // <--- ۶. اگر موفق بود، از Url و FileName داخل نتیجه استفاده کنید
                    product.MainImageUrl = mainImageResult.Url;
                    uploadedFileNames.Add(mainImageResult.FileName); // افزودن به لیست Rollback
                }

                // --- فاز دوم: آپلود گالری (در حلقه) ---
                if (request.Images != null && request.Images.Any())
                {
                    foreach (var imageFile in request.Images)
                    {
                        // <--- ۷. نتیجه را در متغیر SingleSaveResult دریافت کنید
                        var galleryResult = await _fileStorageService.SaveFileAsync(imageFile, FileDirectory);

                        // <--- ۸. وضعیت (Status) را چک کنید
                        if (galleryResult.Status != FileValidationStatus.Success)
                        {
                            // <--- ۹. در صورت خطا، یک Exception پرتاب کنید تا کل فرآیند متوقف و Rollback شود
                            throw new ValidationException($"خطا در آپلود فایل گالری '{imageFile.FileName}': {galleryResult.ErrorMessage}");
                        }

                        // <--- ۱۰. اگر موفق بود، از Url و FileName داخل نتیجه استفاده کنید
                        product.Images.Add(new ProductImage { ImageUrl = galleryResult.Url, IsMainImage = false });
                        uploadedFileNames.Add(galleryResult.FileName); // افزودن به لیست Rollback
                    }
                }

                // --- فاز سوم: ذخیره در دیتابیس ---
                _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("محصول '{ProductName}' با شناسه {ProductId} با موفقیت ایجاد شد.", product.Name, product.Id);
                return product.Id;
            }
            catch (Exception ex)
            {
                // --- فاز Rollback ---
                // (این بخش کاملاً درست کار می‌کند چون ما خطاها را به Exception تبدیل کردیم)
                _logger.LogCritical(ex, "خطا در ایجاد محصول '{ProductName}'. شروع عملیات Rollback فایل‌ها.", request.Name);

                foreach (var fileName in uploadedFileNames)
                {
                    await DeleteFileSilentlyAsync(fileName);
                }

                throw; // خطای اصلی را پرتاب کن
            }
        }

        /// <summary>
        /// متد کمکی برای حذف فایل در فاز Rollback
        /// این متد خطاها را می‌گیرد تا خود فرآیند Rollback متوقف نشود
        /// </summary>
        private async Task DeleteFileSilentlyAsync(string fileName)
        {
            try
            {
                // فرض می‌کنیم نام فایل (GUID.webp) را داریم
                await _fileStorageService.DeleteFileAsync(fileName, FileDirectory);
                _logger.LogWarning("فایل {FileName} با موفقیت (Rollback) حذف شد.", fileName);
            }
            catch (Exception deleteEx)
            {
                // اگر حذف هم خطا داد، فقط لاگ می‌کنیم
                _logger.LogError(deleteEx, "خطا در هنگام Rollback (حذف) فایل {FileName}.", fileName);
            }
        }
    }
}
