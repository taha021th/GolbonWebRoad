using FluentValidation;
using GolbonWebRoad.Application.Exceptions; // using برای NotFoundException
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public int Id { get; set; }

    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه محصول نمی تواند خالی باشد.");
        }
    }
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger; // ۲. ILogger را تعریف کنید
        private readonly IFileStorageService _fileStorageService;

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fileStorageService=fileStorageService;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع فرآیند حذف محصول با شناسه {ProductId}", request.Id);

            // ۱. محصول را به همراه لیست تصاویرش از دیتابیس بخوان
            var productToDelete = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, joinImages: true);

            if (productToDelete == null)
            {
                throw new NotFoundException($"محصول با شناسه {request.Id} برای حذف یافت نشد.");
            }

            // ۲. لیست آدرس تصاویر را قبل از حذف، در یک متغیر ذخیره کن
            var imagesToDelete = productToDelete.Images.Select(i => i.ImageUrl).ToList();

            try
            {
                // ۳. محصول را از ریپازیتوری حذف کن
                await _unitOfWork.ProductRepository.DeleteAsync(productToDelete.Id);

                // ۴. تغییرات را در دیتابیس ذخیره کن
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("محصول با شناسه {ProductId} از دیتابیس با موفقیت حذف شد.", request.Id);

                // ۵. حالا که حذف از دیتابیس موفقیت‌آمیز بود، فایل‌های فیزیکی را پاک کن
                foreach (var imageUrl in imagesToDelete)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        await _fileStorageService.DeleteFileAsync(Path.GetFileName(imageUrl), "products");
                        _logger.LogInformation("فایل تصویر {ImageUrl} از سرور حذف شد.", imageUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "خطای بحرانی در هنگام حذف محصول با شناسه {ProductId}", request.Id);
                throw;
            }
        }
    }
}