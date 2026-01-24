using AutoMapper;
using GolbonWebRoad.Application.Dtos.Faqs;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Faqs.Queries
{
    public class GetFaqsQuery : IRequest<List<FaqDto>>
    {
        public bool OnlyActive { get; set; } = true;
    }

    public class GetFaqsQueryHandler : IRequestHandler<GetFaqsQuery, List<FaqDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetFaqsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public async Task<List<FaqDto>> Handle(GetFaqsQuery request, CancellationToken cancellationToken)
        {
            var list = await _uow.FaqRepository.GetAllAsync(request.OnlyActive);
            return _mapper.Map<List<FaqDto>>(list);
        }
    }
}
