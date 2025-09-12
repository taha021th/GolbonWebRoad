using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IEnumerable<Category>>
    {
        public bool? JoinProducts { get; set; }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<Category>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoriesQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات با پارامترهای ورودی
            _logger.LogInformation("شروع فرآیند دریافت لیست دسته‌بندی‌ها. JoinProducts: {JoinProducts}", request.JoinProducts);

            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync(request.JoinProducts);

                if (categories == null || !categories.Any())
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("هیچ دسته‌بندی در سیستم یافت نشد.");
                    return new List<Category>(); // همیشه یک لیست خالی برگردانید، نه null
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("تعداد {CategoryCount} دسته‌بندی با موفقیت از دیتابیس دریافت شد.", categories.Count());


                return categories;
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت لیست دسته‌بندی‌ها از دیتابیس.");
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}