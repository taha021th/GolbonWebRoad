using FluentValidation;
using GolbonWebRoad.Application.Exceptions; // using برای NotFoundException
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("شناسه دسته بندی نمی تواند خالی باشد.");
        }
    }
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger; // ۲. ILogger را تعریف کنید
        private readonly IFileStorageService _fileStorageService;
        // ۳. ILogger را تزریق کرده و وابستگی اضافی به IMapper را حذف کنید
        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند حذف دسته‌بندی با شناسه {CategoryId}.", request.Id);

            try
            {
                // ۴. ابتدا دسته‌بندی را پیدا کنید تا از وجود آن مطمئن شوید
                var categoryToDelete = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (categoryToDelete == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار
                    _logger.LogWarning("دسته‌بندی با شناسه {CategoryId} برای حذف یافت نشد.", request.Id);
                    throw new NotFoundException($"دسته‌بندی با شناسه {request.Id} یافت نشد.");
                }
                if (categoryToDelete.ImageUrl!=null)
                {
                    _logger.LogInformation("تصویر دسته بندی با شناسه {CategoryId} حذف شد.", categoryToDelete.Id);
                    await _fileStorageService.DeleteFileAsync(Path.GetFileName(categoryToDelete.ImageUrl), "categories");
                }

                await _unitOfWork.CategoryRepository.DeleteAsync(request.Id);
                await _unitOfWork.CompleteAsync();


                _logger.LogInformation("دسته‌بندی با شناسه {CategoryId} و نام '{CategoryName}' با موفقیت حذف شد.",
                    request.Id, categoryToDelete.Name);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام حذف دسته‌بندی با شناسه {CategoryId}.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}