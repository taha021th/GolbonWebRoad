using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Colors;
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
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int Quantity { get; set; }
        public string? SKU { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public List<ColorInputDto> Colors { get; set; } = new();
        public List<IFormFile> NewImages { get; set; } = new();
        public List<string> ImagesToDelete { get; set; } = new();
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام محصول نمی تواند خالی باشد.").MaximumLength(100).WithMessage("نام محصول نباید بیش از 100 کاراکتر باشد.");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("قیمت باید بیشتر از 0 باشد.");
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;


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

            // ۱. محصول مورد نظر را با تمام روابط لازم از دیتابیس بخوان
            var productToUpdate = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id,
                joinImages: true,
                joinColors: true);

            if (productToUpdate == null)
            {
                throw new NotFoundException($"محصول با شناسه {request.Id} یافت نشد.");
            }

            try
            {
                // ۲. نگاشت پراپرتی‌های ساده از Command به Entity
                _mapper.Map(request, productToUpdate);

                // ۳. مدیریت حذف تصاویر
                if (request.ImagesToDelete != null && request.ImagesToDelete.Any())
                {
                    foreach (var imageUrl in request.ImagesToDelete)
                    {
                        var imageToRemove = productToUpdate.Images.FirstOrDefault(i => i.ImageUrl == imageUrl);
                        if (imageToRemove != null)
                        {
                            productToUpdate.Images.Remove(imageToRemove);
                            await _fileStorageService.DeleteFileAsync(imageUrl, "products");
                            _logger.LogInformation("تصویر {ImageUrl} حذف شد.", imageUrl);
                        }
                    }
                }

                // ۴. مدیریت افزودن تصاویر جدید
                if (request.NewImages != null && request.NewImages.Any())
                {
                    foreach (var imageFile in request.NewImages)
                    {
                        var newImageUrl = await _fileStorageService.SaveFileAsync(imageFile, "products");
                        productToUpdate.Images.Add(new ProductImages
                        {
                            ImageUrl = newImageUrl,
                            // اگر محصول هیچ تصویر اصلی ندارد، این را اصلی قرار بده
                            IsMainImage = !productToUpdate.Images.Any(i => i.IsMainImage)
                        });
                    }
                }

                // ۵. مدیریت رنگ‌ها (رویکرد ساده: پاک کردن و افزودن مجدد)
                productToUpdate.ProductColors.Clear(); // <- حذف همه رنگ‌های قبلی
                if (request.Colors != null)
                {
                    foreach (var colorInput in request.Colors.Where(c => !string.IsNullOrWhiteSpace(c.Name)))
                    {
                        var trimmedColorName = colorInput.Name.Trim();
                        var existingColor = await _unitOfWork.ColorRepository.FindByNameAsync(trimmedColorName);

                        if (existingColor == null)
                        {
                            existingColor = new Color { Name = trimmedColorName, HexCode = colorInput.HexCode?.Trim() };
                            await _unitOfWork.ColorRepository.AddAsync(existingColor);
                        }
                        productToUpdate.ProductColors.Add(new ProductColor { Color = existingColor });
                    }
                }

                // ۶. ذخیره تمام تغییرات در یک تراکنش واحد
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("محصول با شناسه {ProductId} با موفقیت ویرایش شد.", productToUpdate.Id);


            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "خطای بحرانی در هنگام ویرایش محصول با شناسه {ProductId}", request.Id);
                throw;
            }

        }
    }
}