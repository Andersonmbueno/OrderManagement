using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderListItemDto>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<OrderListItemDto>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = orders.Select(order => new OrderListItemDto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CreatedAt,
            order.TotalAmount)).ToList();

        return new PagedResult<OrderListItemDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }
}
