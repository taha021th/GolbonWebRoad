using AutoMapper;
using FluentValidation;
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
            _logger.LogInformation("شروع فرآیند ایجاد محصول جدید با نام '{ProductName}'.", request.Name);

            try
            {
                var product = _mapper.Map<Product>(request);

                // Handle main image upload (separate uploader)
                if (request.MainImage != null)
                {
                    var mainUrl = await _fileStorageService.SaveFileAsync(request.MainImage, "products");
                    product.MainImageUrl = mainUrl;
                }

                if (request.Images != null && request.Images.Any())
                {
                    _logger.LogInformation("در حال آپلود {ImageCount} تصویر.", request.Images.Count);
                    bool isFirstImage = true;
                    foreach (var imageFile in request.Images)
                    {
                        var imageUrl = await _fileStorageService.SaveFileAsync(imageFile, "products");
                        product.Images.Add(new ProductImage
                        {
                            ImageUrl = imageUrl,
                            IsMainImage = isFirstImage
                        });
                        isFirstImage = false;
                    }
                }

                // Removed legacy color binding; attributes/variants handle options now
                _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("محصول '{ProductName}' با شناسه {ProductId} با موفقیت ایجاد شد.", product.Name, product.Id);
                return product.Id;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "خطای بحرانی در هنگام ایجاد محصول با نام '{ProductName}'.", request.Name);
                throw;
            }
        }
    }
}