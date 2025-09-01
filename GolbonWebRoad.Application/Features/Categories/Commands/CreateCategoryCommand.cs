using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Interfaces;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام دسته بندی نمی تواند خالی باشد")
                .MaximumLength(100).WithMessage("نام دسته بندی نمی تواند بیشتر از 100 کاراکتر باشد.");
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCategoryCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند ایجاد دسته‌بندی جدید با نام '{CategoryName}'.", request.Name);

            try
            {
                var categoryEntity = _mapper.Map<Category>(request);

                // ۴. ✅ رفع باگ: استفاده از متد AddAsync به جای Update
                var newCategory = _unitOfWork.CategoryRepository.Add(categoryEntity);
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("دسته‌بندی '{CategoryName}' با شناسه {CategoryId} با موفقیت در دیتابیس ایجاد شد.",
                    newCategory.Name, newCategory.Id);

                return _mapper.Map<CategoryDto>(newCategory);
            }
            catch (Exception ex)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام ایجاد دسته‌بندی با نام '{CategoryName}'.", request.Name);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}