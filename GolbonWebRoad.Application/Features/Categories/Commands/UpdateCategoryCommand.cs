using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty()
                .WithMessage("شناسه دسته بندی نمی تواند خالی باشد.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("نام دسته بندی نمی تواند خالی باشد");
        }

    }
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryDto = _mapper.Map<Category>(request);
            var category = await _categoryRepository.UpdateAsync(categoryDto);
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
