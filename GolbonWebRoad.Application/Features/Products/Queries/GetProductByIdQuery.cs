using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
        public bool? JoinCategory { get; set; }
        public bool? JoinReviews { get; set; }
        public bool? JoinImages { get; set; }
        public bool? JoinBrand { get; set; }

    }

    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشه.");
        }
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند دریافت محصول با شناسه {ProductId}.", request.Id);

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, joinCategory: request.JoinCategory, joinImages: request.JoinImages, joinBrand: request.JoinBrand, joinReviews: request.JoinReviews);

                if (product == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("محصول با شناسه {ProductId} در دیتابیس یافت نشد.", request.Id);
                    return null; // کنترلر این null را به 404 Not Found تبدیل می‌کند
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("محصول با شناسه {ProductId} و نام '{ProductName}' با موفقیت یافت شد.", request.Id, product.Name);


                return product;
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت محصول با شناسه {ProductId} از دیتابیس.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}