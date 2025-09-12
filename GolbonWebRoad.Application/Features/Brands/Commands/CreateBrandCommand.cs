using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Brands.Commands
{
    public class CreateBrandCommand : IRequest
    {
        public string Name { get; set; }
    }
    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateBrandCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork= unitOfWork;
            _mapper= mapper;
        }
        public async Task Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Brand>(request);
            await _unitOfWork.BrandRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
