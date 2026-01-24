using AutoMapper;
using GolbonWebRoad.Application.Dtos.Faqs;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Faqs.Queries
{
    public class GetFaqCategoriesQuery : IRequest<List<FaqCategoryDto>>
    {
        public bool OnlyActive { get; set; } = true;
    }

    public class GetFaqCategoriesQueryHandler : IRequestHandler<GetFaqCategoriesQuery, List<FaqCategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetFaqCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }
        public async Task<List<FaqCategoryDto>> Handle(GetFaqCategoriesQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FaqCategoryRepository.GetAllAsync(request.OnlyActive);
            return _mapper.Map<List<FaqCategoryDto>>(list);
        }
    }
}
