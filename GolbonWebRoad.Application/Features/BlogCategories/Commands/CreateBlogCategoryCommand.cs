using AutoMapper;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogCategories.Commands
{
    public class CreateBlogCategoryCommand : IRequest<BlogCategory>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slog { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltTextImageUrl { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaRobots { get; set; }
        public string? CanonicalUrl { get; set; }
    }
    public class CreateBlogCategoryCommandHandler : IRequestHandler<CreateBlogCategoryCommand, BlogCategory>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBlogCategoryCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public CreateBlogCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateBlogCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _logger=logger;
            _fileStorageService=fileStorageService;
        }
        public async Task<BlogCategory> Handle(CreateBlogCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع ایجاد دسته بندی بلاگ جدید: {BlogCategoryName}", request.Name);
            var entity = _mapper.Map<BlogCategory>(request);
            if (request.Image!=null)
            {
                var saved = await _fileStorageService.SaveFileAsync(request.Image, "blogCategories");
                entity.ImageUrl=saved.Url;
            }
            await _unitOfWork.BlogCategoryRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("دسته بندی بلاگ {BlogCategoryName} با شناسه {BlogCategoryId} ایجاد شد.", entity.Name, entity.Id);
            return entity;
        }
    }
}
