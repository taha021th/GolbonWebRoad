using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Brands.Commands
{
    public class CreateBrandCommand : IRequest<Brand>
    {
        public string Name { get; set; }
        public string? Slog { get; set; }
        public string Content { get; set; }

        public IFormFile? Image { get; set; }
    }

    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام برند نمیتواند خالی باشد")
                .MaximumLength(100).WithMessage("نام برند نمیتواند بیشتر از 100 کاراکتر باشد.");
        }
    }

    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Brand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBrandCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public CreateBrandCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateBrandCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }
        public async Task<Brand> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع ایجاد برند جدید: {BrandName}", request.Name);

            var entity = _mapper.Map<Brand>(request);
            if (request.Image != null)
            {
                var saved = await _fileStorageService.SaveFileAsync(request.Image, "brands");
                entity.ImageUrl = saved.Url;
            }

            await _unitOfWork.BrandRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("برند {BrandName} با شناسه {BrandId} ایجاد شد", entity.Name, entity.Id);
            return entity;
        }
    }
}
