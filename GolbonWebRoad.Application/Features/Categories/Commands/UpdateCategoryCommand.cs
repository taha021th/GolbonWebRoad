using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Application.Exceptions; // using برای NotFoundException
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Slog { get; set; }
    }

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty()
                .WithMessage("شناسه دسته بندی نمی تواند خالی باشد.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("نام دسته بندی نمی تواند خالی باشد")
                .MaximumLength(100).WithMessage("نام دسته بندی نمی تواند بیشتر از 100 کاراکتر باشد.");
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger; // ۲. ILogger را تعریف کنید

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateCategoryCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند به‌روزرسانی دسته‌بندی با شناسه {CategoryId}.", request.Id);

            try
            {
                // ۴. ✅ رفع باگ: ابتدا موجودیت را از دیتابیس بخوانید
                var categoryToUpdate = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (categoryToUpdate == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار
                    _logger.LogWarning("دسته‌بندی با شناسه {CategoryId} برای به‌روزرسانی یافت نشد.", request.Id);
                    throw new NotFoundException($"دسته‌بندی با شناسه {request.Id} یافت نشد.");
                }

                var oldName = categoryToUpdate.Name;

                // ۵. ✅ رفع باگ: اطلاعات جدید را روی موجودیت خوانده شده کپی کنید
                _mapper.Map(request, categoryToUpdate);

                _unitOfWork.CategoryRepository.Update(categoryToUpdate); // استفاده از متد آسنکرون
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation(
                    "دسته‌بندی با شناسه {CategoryId} از نام '{OldName}' به '{NewName}' با موفقیت به‌روزرسانی شد.",
                    request.Id, oldName, request.Name);

                return _mapper.Map<CategoryDto>(categoryToUpdate);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام به‌روزرسانی دسته‌بندی با شناسه {CategoryId}.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}