using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Brands.Queries
{
    public class GetBrandsQuery : IRequest<IEnumerable<Brand>>
    {

    }
    public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, IEnumerable<Brand>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetBrandsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Brand>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();
            return brands;
        }
    }
}
