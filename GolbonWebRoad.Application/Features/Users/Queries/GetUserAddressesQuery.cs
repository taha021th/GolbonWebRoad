using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Users.Queries
{
    public class GetUserAddressesQuery : IRequest<IEnumerable<UserAddress>>
    {
        public string UserId { get; set; }
    }

    public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, IEnumerable<UserAddress>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetUserAddressesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserAddress>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId)) return Enumerable.Empty<UserAddress>();
            return await _unitOfWork.UserAddressRepository.GetByUserIdAsync(request.UserId);
        }
    }
}


