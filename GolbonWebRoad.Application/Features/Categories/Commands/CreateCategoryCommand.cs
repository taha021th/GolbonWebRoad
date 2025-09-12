using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Interfaces.Services;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // ۱. این using را برای دسترسی به ILogger اضافه کنید

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest
    {
        public string Name { get; set; }
        public string? Slog { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام دسته بندی نمی تواند خالی باشد")
                .MaximumLength(100).WithMessage("نام دسته بندی نمی تواند بیشتر از 100 کاراکتر باشد.");
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCategoryCommandHandler> logger, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService=fileStorageService;
        }

        public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("شروع فرآیند ایجاد دسته‌بندی جدید با نام '{CategoryName}'.", request.Name);

            try
            {
                var categoryEntity = _mapper.Map<Category>(request);
                if (request.Image != null)
                {
                    var imageUrl = await _fileStorageService.SaveFileAsync(request.Image, "categories");
                    categoryEntity.ImageUrl = imageUrl;
                }


                var newCategory = _unitOfWork.CategoryRepository.Add(categoryEntity);
                await _unitOfWork.CompleteAsync();


                _logger.LogInformation("دسته‌بندی '{CategoryName}' با شناسه {CategoryId} با موفقیت در دیتابیس ایجاد شد.",
                    newCategory.Name, newCategory.Id);


            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex, "خطای بحرانی در هنگام ایجاد دسته‌بندی با نام '{CategoryName}'.", request.Name);
                throw;
            }
        }
    }
}