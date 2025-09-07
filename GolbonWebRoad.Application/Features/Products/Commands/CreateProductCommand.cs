using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Slog { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام محصول نمی تواند خالی باشد.")
                .MaximumLength(100).WithMessage("نام محصول نمی تواند بیشتر از 100 کاراکتر باشد.");
            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
        }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات با پارامترهای کلیدی
            _logger.LogInformation("شروع فرآیند ایجاد محصول جدید با نام '{ProductName}' در دسته‌بندی {CategoryId}.",
                request.Name, request.CategoryId);

            try
            {
                var product = _mapper.Map<Product>(request);

                // ۴. استفاده از متد آسنکرون برای عملیات دیتابیس
                var newProduct = _unitOfWork.ProductRepository.Add(product);
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("محصول '{ProductName}' با شناسه {ProductId} با موفقیت در دیتابیس ایجاد شد.",
                    newProduct.Name, newProduct.Id);

                return _mapper.Map<ProductDto>(newProduct);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام ایجاد محصول با نام '{ProductName}'.", request.Name);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}