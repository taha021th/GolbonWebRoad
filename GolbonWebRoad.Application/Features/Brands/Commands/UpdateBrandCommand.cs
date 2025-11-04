using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Brands.Commands
{
    public class UpdateBrandCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ExistingImage { get; set; }
        public IFormFile? NewImage { get; set; }
    }

    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("شناسه معتبر نیست");
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام الزامی است").MaximumLength(100);
        }
    }

    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBrandCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        public UpdateBrandCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateBrandCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بهروزرسانی برند {BrandId}", request.Id);
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(request.Id, false);
            if (brand == null)
            {
                _logger.LogWarning("برند {BrandId} پیدا نشد", request.Id);
                throw new NotFoundException($"برند با شناسه {request.Id} یافت نشد.");
            }

            var oldName = brand.Name;
            brand.Name = request.Name;

            if (request.NewImage != null)
            {
                if (!string.IsNullOrWhiteSpace(request.ExistingImage))
                {
                    await _fileStorageService.DeleteFileAsync(System.IO.Path.GetFileName(request.ExistingImage), "brands");
                }
                var saved = await _fileStorageService.SaveFileAsync(request.NewImage, "brands");
                brand.ImageUrl = saved.Url;
            }

            _unitOfWork.BrandRepository.Update(brand);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("برند {BrandId} از '{Old}' به '{New}' بهروزرسانی شد", request.Id, oldName, brand.Name);
        }
    }
}
