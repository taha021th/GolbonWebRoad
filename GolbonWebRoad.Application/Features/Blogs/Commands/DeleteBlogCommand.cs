using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Blogs.Commands
{
    public class DeleteBlogCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBlogCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public DeleteBlogCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteBlogCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task Handle(DeleteBlogCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("دریافت بلاگ با شناسه {BlogId} برای حذف.", request.Id);
            var blogEntity = await _unitOfWork.BlogRepository.GetByIdAsync(request.Id, false, false);
            _logger.LogInformation("حذف بلاگ با شناسه {BlogId}", request.Id);
            _unitOfWork.BlogRepository.Delete(blogEntity);
            _logger.LogInformation("حذف تصویر بلاگ با شناسه {BlogId}", request.Id);
            await _fileStorageService.DeleteFileAsync(blogEntity.MainImageUrl, "blogs");
            await _unitOfWork.CompleteAsync();
        }
    }
}
