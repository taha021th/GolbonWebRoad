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
        public List<IFormFile> NewImages { get; set; } = new();
        public List<string> ImagesToDelete { get; set; } = new();
        public List<ColorInputDto> NewColors { get; set; } = new();
        public List<int> ColorsToDelete { get; set; } = new();

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

                #region Upload Images
                // ۳. مدیریت حذف تصاویر
                if (request.ImagesToDelete != null && request.ImagesToDelete.Any())
                {
                    foreach (var imageUrl in request.ImagesToDelete)
                    {
                        var imageToRemove = productToUpdate.Images.FirstOrDefault(i => i.ImageUrl == imageUrl);
                        if (imageToRemove != null)
                        {
                            productToUpdate.Images.Remove(imageToRemove);
                            await _fileStorageService.DeleteFileAsync(Path.GetFileName(imageUrl), "products");
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
                #endregion


                #region Colors
                if (request.ColorsToDelete != null && request.ColorsToDelete.Any())
                {
                    // لیستی از آیتم‌های ProductColor که باید حذف شوند را پیدا کن
                    var colorsToRemove = productToUpdate.ProductColors
                        .Where(pc => request.ColorsToDelete.Contains(pc.ColorId))
                        .ToList();

                    foreach (var productColor in colorsToRemove)
                    {
                        productToUpdate.ProductColors.Remove(productColor);
                    }
                }

                // ب) افزودن رنگ‌های جدید
                if (request.NewColors != null && request.NewColors.Any())
                {
                    foreach (var newColor in request.NewColors.Where(c => !string.IsNullOrWhiteSpace(c.Name)))
                    {
                        var trimmedColorName = newColor.Name.Trim();

                        // چک کن که این رنگ از قبل به محصول اضافه نشده باشد
                        if (productToUpdate.ProductColors.Any(pc => pc.Color.Name == trimmedColorName))
                        {
                            continue; // اگر بود، برو سراغ رنگ بعدی
                        }

                        var existingColor = await _unitOfWork.ColorRepository.FindByNameAsync(trimmedColorName);

                        if (existingColor == null) // اگر رنگ در دیتابیس نبود، بسازش
                        {
                            existingColor = new Color { Name = trimmedColorName, HexCode = newColor.HexCode?.Trim() };
                            await _unitOfWork.ColorRepository.AddAsync(existingColor);
                        }

                        // رابطه جدید را به محصول اضافه کن
                        productToUpdate.ProductColors.Add(new ProductColor { Color = existingColor });
                    }
                }
                #endregion

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