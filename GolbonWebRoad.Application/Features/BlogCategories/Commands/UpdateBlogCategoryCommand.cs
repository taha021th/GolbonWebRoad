using AutoMapper;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogCategories.Commands
{
    public class UpdateBlogCategoryCommand : IRequest<BlogCategory>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slog { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string? AltTextImageUrl { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaRobots { get; set; }
        public string? CanonicalUrl { get; set; }
    }
    public class UpdateBlogCategoryCommandHandler : IRequestHandler<UpdateBlogCategoryCommand, BlogCategory>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBlogCategoryCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public UpdateBlogCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateBlogCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _logger=logger;
            _fileStorageService=fileStorageService;
        }

        public async Task<BlogCategory> Handle(UpdateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع بروزرسانی دسته بندی بلاگ {BlogCategory}", request.Id);
            var entity = await _unitOfWork.BlogCategoryRepository.GetByIdAsync(request.Id);
            if (entity==null)
            {
                _logger.LogWarning("دسته بندی بلاگ {BlogCategoryId} پیدا نشد", request.Id);
                throw new NotFoundException($"دسته بندی بلاگ با شناسه {request.Id} یافت نشد.");
            }
            var oldName = entity.Name;
            entity=_mapper.Map<BlogCategory>(request);

            if (request.Image !=null)
            {
                await _fileStorageService.DeleteFileAsync(entity.ImageUrl, "blogCategories");
                var saved = await _fileStorageService.SaveFileAsync(request.Image, "blogCategories");

                entity.ImageUrl = saved.Url;
            }
            _unitOfWork.BlogCategoryRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("دسته بندی بلاگ {BlogCategoryName} با شناسه {BlogCategoryId} ایجاد شد.", entity.Name, entity.Id);
            return entity;
        }
    }
}
