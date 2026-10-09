using FluentAssertions;
using Moq;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using Xunit;

namespace OrderManagement.UnitTests;

public sealed class CancelOrderHandlerTests
{
    [Fact]
    public async Task Should_Cancel_Pending_Order()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            new[] { new OrderItem("Keyboard", 1, 100m) });

        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new CancelOrderHandler(repository.Object);

        await handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        order.Status.Should().Be(OrderStatus.Cancelled);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Reject_Cancellation_When_Order_Is_Not_Pending()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            new[] { new OrderItem("Keyboard", 1, 100m) });
        order.Cancel();

        var repository = new Mock<IOrderRepository>();
        repository.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new CancelOrderHandler(repository.Object);

        var act = () => handler.Handle(
            new CancelOrderCommand(order.Id),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only pending orders can be cancelled.");
    }

    [Fact]
    public async Task Should_Throw_When_Order_Does_Not_Exist()
    {
        var repository = new Mock<IOrderRepository>();
        var orderId = Guid.NewGuid();

        repository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new CancelOrderHandler(repository.Object);

        var act = () => handler.Handle(
            new CancelOrderCommand(orderId),
            CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
