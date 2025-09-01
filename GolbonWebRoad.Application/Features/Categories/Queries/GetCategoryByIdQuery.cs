using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public int Id { get; set; }
        public bool? JoinProducts { get; set; }
    }

    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("شناسه دسته بندی نمی تواند خالی باشد");
        }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoryByIdQueryHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoryByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات با پارامترهای ورودی
            _logger.LogInformation(
                "شروع فرآیند دریافت دسته‌بندی با شناسه {CategoryId}. JoinProducts: {JoinProducts}",
                request.Id, request.JoinProducts);

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id, request.JoinProducts);

                if (category == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار که خطا نیست
                    _logger.LogWarning("دسته‌بندی با شناسه {CategoryId} در دیتابیس یافت نشد.", request.Id);
                    return null; // کنترلر این null را به 404 Not Found تبدیل می‌کند
                }

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز
                _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} و نام '{CategoryName}' با موفقیت یافت شد.",
                    request.Id, category.Name);

                var categoryDto = _mapper.Map<CategoryDto>(category);
                return categoryDto;
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده (مثلاً خطای دیتابیس)
                _logger.LogCritical(ex, "خطای بحرانی در هنگام دریافت دسته‌بندی با شناسه {CategoryId} از دیتابیس.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}