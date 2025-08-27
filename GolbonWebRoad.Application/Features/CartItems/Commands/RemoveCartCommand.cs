using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.CartItems.Commands
{
    public class RemoveCartCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
    public class RemoveCartCommandHandler : IRequestHandler<RemoveCartCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoveCartCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _unitOfWork.CartItemRepository.GetCartItemAsync(request.UserId, request.ProductId);
            if (cartItem != null) throw new NotFoundException("آیتم مورد نظر در سبر خرید یافت نشد.");
            _unitOfWork.CartItemRepository.RemoveCartItem(cartItem);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;

        }
    }
}
