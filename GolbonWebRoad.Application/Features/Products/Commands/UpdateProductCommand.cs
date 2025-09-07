using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class UpdateProductCommand : IRequest
    {
        public int Id { get; set; }
        public string Slog { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
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
        private readonly ILogger<UpdateProductCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات با پارامترهای کلیدی
            _logger.LogInformation("شروع فرآیند به‌روزرسانی محصول با شناسه {ProductId}.", request.Id);

            try
            {
                var productToUpdate = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
                if (productToUpdate == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("محصول با شناسه {ProductId} برای به‌روزرسانی یافت نشد.", request.Id);
                    throw new NotFoundException("محصولی با این شناسه یافت نشد.");
                }

                _mapper.Map(request, productToUpdate);

                // ۴. استفاده از متد آسنکرون برای عملیات دیتابیس
                _unitOfWork.ProductRepository.Update(productToUpdate);
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("محصول با شناسه {ProductId} و نام '{ProductName}' با موفقیت به‌روزرسانی شد.",
                    productToUpdate.Id, productToUpdate.Name);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام به‌روزرسانی محصول با شناسه {ProductId}.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}