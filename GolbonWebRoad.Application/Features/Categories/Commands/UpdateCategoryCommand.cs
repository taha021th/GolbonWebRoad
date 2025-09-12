using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Exceptions; // using برای NotFoundException
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Slog { get; set; }
        public string? ExistingImage { get; set; }
        public IFormFile? NewImage { get; set; }
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

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService=fileStorageService;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند به‌روزرسانی دسته‌بندی با شناسه {CategoryId}.", request.Id);

            try
            {

                var categoryToUpdate = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (categoryToUpdate == null)
                {

                    _logger.LogWarning("دسته‌بندی با شناسه {CategoryId} برای به‌روزرسانی یافت نشد.", request.Id);
                    throw new NotFoundException($"دسته‌بندی با شناسه {request.Id} یافت نشد.");
                }

                var oldName = categoryToUpdate.Name;


                _mapper.Map(request, categoryToUpdate);

                // استفاده از متد آسنکرون



                _logger.LogInformation(
                    "دسته‌بندی با شناسه {CategoryId} از نام '{OldName}' به '{NewName}' با موفقیت به‌روزرسانی شد.",
                    request.Id, oldName, request.Name);

                if (request.NewImage != null)
                {
                    await _fileStorageService.DeleteFileAsync(Path.GetFileName(request.ExistingImage), "categories");
                    _logger.LogInformation("تصویر {ImageUrl} حذف شد.", request.ExistingImage);
                    var newImageUrl = await _fileStorageService.SaveFileAsync(request.NewImage, "categories");
                    categoryToUpdate.ImageUrl=newImageUrl;
                }
                else
                {
                    categoryToUpdate.ImageUrl=categoryToUpdate.ImageUrl;
                }
                _unitOfWork.CategoryRepository.Update(categoryToUpdate);
                await _unitOfWork.CompleteAsync();
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