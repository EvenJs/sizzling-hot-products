using FluentAssertions;

using NSubstitute;

using Application.Interfaces;
using Application.Tests.Builders;
using Application.Services;

using Domain;



namespace Application.Tests;

public class SizzlingHotServiceTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public SizzlingHotServiceTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _productRepository = Substitute.For<IProductRepository>();
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_ReturnsProductWithMostSales()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("Measure").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 1)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O2").WithEntries([new OrderEntry("P1",1)]).WithCustomerId("C2").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O3").WithEntries([new OrderEntry("P2",1)]).WithCustomerId("C3").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("Hammer");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_CountsProductOncePerOrder_RegardlessOfQuantity()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("Measure").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 5)]).WithCustomerId("C1").Build(),
            new OrderBuilder().WithOrderId("O3").WithEntries([new OrderEntry("P2",1)]).WithCustomerId("C1").Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("Hammer");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_DeduplicatesSalesForSameCustomerOnSameDay()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("Measure").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 1)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O2").WithEntries([new OrderEntry("P1",2)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O3").WithEntries([new OrderEntry("P2", 1)]).WithCustomerId("C2").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O4").WithEntries([new OrderEntry("P2", 1)]).WithCustomerId("C3").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("Measure");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_ExcludesCancelledOrders()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("Measure").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 1)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O2").WithEntries([new OrderEntry("P1",2)]).WithCustomerId("C2").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O1").WithStatus(OrderStatus.Cancelled).Build(),
            new OrderBuilder().WithOrderId("O3").WithEntries([new OrderEntry("P2", 2)]).WithCustomerId("C3").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O4").WithEntries([new OrderEntry("P2", 1)]).WithCustomerId("C4").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("Measure");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_SelectsAlphabeticallyFirstProduct_WhenSalesAreTied()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 5)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O2").WithEntries([new OrderEntry("P2",1)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("BBQ");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_IgnoresCancellation_WhenNoMatchingCompletedOrder()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 5)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O2").WithStatus(OrderStatus.Cancelled).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        var topProduct = result.Daily.First();
        topProduct.Product.Name.Should().Be("Hammer");
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_OmitsDay_WhenAllOrdersForThatDayAreCancelled()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 5)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O1").WithStatus(OrderStatus.Cancelled).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Assert
        result.Daily.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Period_IsNull_WhenAllOrdersAreCancelled()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P1", 5)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
            new OrderBuilder().WithOrderId("O1").WithStatus(OrderStatus.Cancelled).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Result 
        result.Period.Should().BeNull();
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_ReturnsEmpty_WhenOrdersListIsEmpty()
    {
        // Arrange
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>();

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Result 
        result.Daily.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_ReturnsEmpty_WhenProductsListIsEmpty()
    {
        // Arrange
        var products = new List<Product>();

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Result 
        result.Daily.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSizzlingHotAsync_Daily_SkipsEntry_WhenProductIdNotFoundInProductsList()
    {
        // Arrange 
        var products = new List<Product>
        {
            new ProductBuilder().WithId("P1").WithName("Hammer").Build(),
            new ProductBuilder().WithId("P2").WithName("BBQ").Build(),
        };

        var orders = new List<Order>
        {
            new OrderBuilder().WithOrderId("O1").WithEntries([new OrderEntry("P99", 5)]).WithCustomerId("C1").WithDate(new DateOnly(2026, 4, 21)).Build(),
        };

        var fromDate = new DateOnly(2026, 4, 21);
        var toDate = new DateOnly(2026, 4, 23);

        _orderRepository.GetAllAsync().Returns(orders);
        _productRepository.GetAllAsync().Returns(products);

        var service = new SizzlingHotService(_orderRepository, _productRepository);

        // Act
        var result = await service.GetSizzlingHotAsync(fromDate, toDate);

        // Result 
        result.Daily.Should().BeEmpty();
    }
}