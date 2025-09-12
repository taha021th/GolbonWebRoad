using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Brands.Commands
{
    public class UpdateBrandCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateBrandCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Brand>(request);
            _unitOfWork.BrandRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
