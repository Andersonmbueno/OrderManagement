using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();

    // Private constructor is used by EF Core when loading an existing order.
    private Order() { }

    private Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // The collection is exposed as read-only to protect the aggregate boundary.
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Business rule: TotalAmount belongs to the domain, not the application layer.
    public decimal TotalAmount => _items.Sum(item => item.UnitPrice * item.Quantity);

    public static Order Create(Guid customerId, IEnumerable<OrderItem> items)
    {
        var order = new Order(customerId);
        order._items.AddRange(items);

        if (order._items.Count == 0)
            throw new InvalidOperationException("An order must contain at least one item.");

        return order;
    }

    public void Cancel()
    {
        // Business rule: only pending orders can be cancelled.
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}
