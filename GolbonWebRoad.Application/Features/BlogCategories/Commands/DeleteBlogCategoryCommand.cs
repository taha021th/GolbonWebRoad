using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogCategories.Commands
{
    public class DeleteBlogCategoryCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteBlogCategoryCommandHandler : IRequestHandler<DeleteBlogCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBlogCategoryCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public DeleteBlogCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteBlogCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task Handle(DeleteBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("حذف دسته بندی بلاگ با شناسه {BlogCategoryId}", request.Id);
            var blogCategory = await _unitOfWork.BlogCategoryRepository.GetByIdAsync(request.Id, false);
            _unitOfWork.BlogCategoryRepository.DeleteAsync(blogCategory);
            await _fileStorageService.DeleteFileAsync(blogCategory.ImageUrl, "blogCategories");
            await _unitOfWork.CompleteAsync();
        }
    }
}