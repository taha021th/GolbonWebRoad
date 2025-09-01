using FluentValidation;
using GolbonWebRoad.Application.Exceptions; // using برای NotFoundException
using GolbonWebRoad.Application.Interfaces;
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

        // ۳. ILogger را از طریق سازنده تزریق کنید
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // لاگ اطلاعاتی: ثبت شروع عملیات
            _logger.LogInformation("شروع فرآیند حذف محصول با شناسه {ProductId}.", request.Id);

            try
            {
                // ۴. ابتدا محصول را پیدا کنید تا از وجود آن مطمئن شوید
                var productToDelete = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id);
                if (productToDelete == null)
                {
                    // لاگ هشدار: ثبت یک نتیجه منفی قابل انتظار
                    _logger.LogWarning("محصول با شناسه {ProductId} برای حذف یافت نشد.", request.Id);
                    throw new NotFoundException($"محصولی با شناسه {request.Id} یافت نشد.");
                }

                await _unitOfWork.ProductRepository.DeleteAsync(request.Id);
                await _unitOfWork.CompleteAsync();

                // لاگ اطلاعاتی: ثبت نتیجه موفقیت‌آمیز عملیات
                _logger.LogInformation("محصول با شناسه {ProductId} و نام '{ProductName}' با موفقیت حذف شد.",
                    request.Id, productToDelete.Name);
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                // لاگ بحرانی: ثبت خطاهای پیش‌بینی نشده
                _logger.LogCritical(ex, "خطای بحرانی در هنگام حذف محصول با شناسه {ProductId}.", request.Id);
                throw; // Exception را دوباره پرتاب کن تا Middleware آن را به 500 تبدیل کند
            }
        }
    }
}