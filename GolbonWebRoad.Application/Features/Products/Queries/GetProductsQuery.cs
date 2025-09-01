using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? SortOrder { get; set; }
        public bool? JoinCategory { get; set; }
    }

    public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
    {
        public GetProductsQueryValidator()
        {
            // در اینجا می‌توانید قوانینی برای فیلترها اضافه کنید
            // مثلاً: RuleFor(p => p.SortOrder).Must(BeAValidSortOrder);
        }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetProductsQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات به همراه تمام پارامترهای ورودی
            _logger.LogInformation(
                "شروع فرآیند دریافت محصولات با فیلترها: SearchTerm='{SearchTerm}', CategoryId={CategoryId}, SortOrder='{SortOrder}', JoinCategory={JoinCategory}",
                request.SearchTerm, request.CategoryId, request.SortOrder, request.JoinCategory);

            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync(request.SearchTerm, request.CategoryId, request.SortOrder, request.JoinCategory);

                if (products == null || !products.Any())
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning(
                        "هیچ محصولی با فیلترهای مشخص شده یافت نشد: SearchTerm='{SearchTerm}', CategoryId={CategoryId}",
                        request.SearchTerm, request.CategoryId);
                    return new List<ProductDto>(); // همیشه یک لیست خالی برگردانید، نه null
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("تعداد {ProductCount} محصول با فیلترهای مشخص شده با موفقیت یافت شد.", products.Count());

                return _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت محصولات از دیتابیس.");
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}