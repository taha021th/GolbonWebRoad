using AutoMapper;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Blogs.Commands
{
    public class UpdateBlogCommand : IRequest<Blog>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ShortDescription { get; set; }
        public string? MainImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string? MainImageAltText { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? Slog { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? H1Title { get; set; }
        public string? MetaRobots { get; set; }
        public int CategoryId { get; set; }
    }
    public class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, Blog>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBlogCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public UpdateBlogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateBlogCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _logger=logger;
            _fileStorageService=fileStorageService;
        }

        public async Task<Blog> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع بروزرسانی بلاگ با شناسه {BlogId}", request.Id);
            var entity = await _unitOfWork.BlogRepository.GetByIdAsync(request.Id, false, false);
            if (entity == null)
            {
                _logger.LogInformation("بلاگ با شناسه {BlogId} یافت نشد.", request.Id);
                throw new NotFoundException($"بلاگ با شناسه {request.Id} یافت نشد.");
            }


            if (entity.IsPublished==false && request.IsPublished==true)
            {
                entity.PublishDate = DateTime.UtcNow;
            }
            entity=_mapper.Map<Blog>(request);

            if (request.Image!=null)
            {
                await _fileStorageService.DeleteFileAsync(entity.MainImageUrl, "blogs");
                var saved = await _fileStorageService.SaveFileAsync(request.Image, "blogs");
                entity.MainImageUrl=saved.Url;
            }

            _unitOfWork.BlogRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("بلاگ با عنوان {BlogTitle} و با شناسه {BlogId} ایجاد شد.", entity.Title, entity.Id);
            return entity;
        }
    }
}

