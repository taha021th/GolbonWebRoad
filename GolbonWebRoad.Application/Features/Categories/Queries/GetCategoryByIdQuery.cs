using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public int Id { get; set; }
        public bool? JoinProducts { get; set; }
    }
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository=categoryRepository;
            _mapper=mapper;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, request.JoinProducts);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }
    }
}
