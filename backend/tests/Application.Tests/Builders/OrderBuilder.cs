using Domain;

namespace Application.Tests.Builders;

public class OrderBuilder
{
    private string _orderId = "01";
    private string _customerId = "C1";
    private DateOnly _date = new(2026, 4, 21);
    private OrderStatus _status = OrderStatus.Completed;
    private List<OrderEntry> _entries = [];

    public OrderBuilder WithOrderId(string orderId)
    {
        _orderId = orderId;
        return this;
    }

    public OrderBuilder WithCustomerId(string customerId)
    {
        _customerId = customerId;
        return this;
    }
    public OrderBuilder WithDate(DateOnly date)
    {
        _date = date;
        return this;
    }
    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }
    public OrderBuilder WithEntries(List<OrderEntry> entries)
    {
        _entries = entries;
        return this;
    }
    public Order Build()
    {
        return new Order(_orderId, _customerId, _date, _status)
        {
            Entries = _entries
        };
    }
}