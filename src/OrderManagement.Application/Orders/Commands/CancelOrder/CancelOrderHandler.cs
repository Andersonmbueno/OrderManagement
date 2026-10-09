using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _repository;

    public CancelOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new KeyNotFoundException($"Order '{request.OrderId}' was not found.");

        // The domain validates whether cancellation is allowed.
        order.Cancel();

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
