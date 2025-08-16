using AutoMapper;
using GolbonWebRoad.Application.Dtos.Categories;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public bool? JoinProducts { get; set; }
    }
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository=categoryRepository;
            _mapper=mapper;

        }
        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(request.JoinProducts);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return categoriesDto;
        }
    }
}
