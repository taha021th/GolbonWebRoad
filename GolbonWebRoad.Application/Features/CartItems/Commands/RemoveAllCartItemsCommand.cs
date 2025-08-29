using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class RemoveAllCartItemsCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
    }


    public class RemoveAllCartItemsCommandHandler : IRequestHandler<RemoveAllCartItemsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoveAllCartItemsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        async Task<Unit> IRequestHandler<RemoveAllCartItemsCommand, Unit>.Handle(RemoveAllCartItemsCommand request, CancellationToken cancellationToken)
        {
            var getCartItems = await _unitOfWork.CartItemRepository.GetCartItemsByUserIdAsync(request.UserId);
            if (getCartItems == null) { throw new NotFoundException("Not found item for user"); }
            _unitOfWork.CartItemRepository.RemoveAllCartItem(getCartItems);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
