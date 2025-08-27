using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class UpdateCartItemCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);
            if (cartItem == null) throw new NotFoundException("آیتم مورد نظر در سبد خرید یافت نشد.");

            if (request.Quantity <= 0)
            {
                _unitOfWork.CartItemRepository.RemoveCartItem(cartItem);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
                _unitOfWork.CartItemRepository.UpdateCartItem(cartItem);
            }

            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

    }
}
