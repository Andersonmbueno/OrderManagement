using FluentAssertions;
using Moq;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using Xunit;

namespace OrderManagement.UnitTests;

public sealed class CreateOrderHandlerTests
{
    [Fact]
    public async Task Should_Create_Order_And_Return_Id()
    {
        var repository = new Mock<IOrderRepository>();
        var handler = new CreateOrderHandler(repository.Object);

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new[]
            {
                new CreateOrderItem("Keyboard", 2, 100m),
                new CreateOrderItem("Mouse", 1, 50m)
            });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
        repository.Verify(r => r.AddAsync(
            It.IsAny<OrderManagement.Domain.Entities.Order>(),
            It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Create_Order_With_Correct_Domain_Total()
    {
        var repository = new Mock<IOrderRepository>();
        OrderManagement.Domain.Entities.Order? capturedOrder = null;

        repository
            .Setup(r => r.AddAsync(It.IsAny<OrderManagement.Domain.Entities.Order>(), It.IsAny<CancellationToken>()))
            .Callback<OrderManagement.Domain.Entities.Order, CancellationToken>((order, _) => capturedOrder = order)
            .Returns(Task.CompletedTask);

        var handler = new CreateOrderHandler(repository.Object);

        await handler.Handle(
            new CreateOrderCommand(
                Guid.NewGuid(),
                new[] { new CreateOrderItem("Notebook", 3, 25.50m) }),
            CancellationToken.None);

        capturedOrder.Should().NotBeNull();
        capturedOrder!.TotalAmount.Should().Be(76.50m);
    }
}
