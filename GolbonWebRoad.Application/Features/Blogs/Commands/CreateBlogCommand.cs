using AutoMapper;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Blogs.Commands
{
    public class CreateBlogCommand : IRequest
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; } // خلاصه مطلب
        public string Content { get; set; } // متن اصلی
        public string? MainImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsPublished { get; set; } // وضعیت انتشار
        public string? ShortDescription { get; set; }

        public string? Slog { get; set; }
        public string? H1Title { get; set; }
        public int ReadTimeMinutes { get; set; }
        public string? MetaTitle { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaRobots { get; set; }
        public int CategoryId { get; set; }
        public string? MainImageAltText { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBlogCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public CreateBlogCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBlogCommandHandler> logger, IMapper mapper, IFileStorageService fileStorageService)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;
            _mapper=mapper;
            _fileStorageService=fileStorageService;
        }

        public async Task Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {

            var entity = _mapper.Map<Blog>(request);
            _logger.LogInformation("شروع ایجاد بلاگ");
            await _unitOfWork.BlogRepository.AddAsync(entity);
            _logger.LogInformation("ذخیره تصویر لاگ");
            var resultSaveImage = await _fileStorageService.SaveFileAsync(request.Image, "blogs");
            entity.MainImageUrl=resultSaveImage.Url;
            await _unitOfWork.CompleteAsync();
        }
    }
}
