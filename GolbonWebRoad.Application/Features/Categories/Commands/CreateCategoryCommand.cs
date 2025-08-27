using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; }
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


    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryDto = _mapper.Map<Category>(request);
            var category = await _unitOfWork.CategoryRepository.AddAsync(categoryDto);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
