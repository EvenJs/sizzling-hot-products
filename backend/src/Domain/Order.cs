namespace Domain;

public sealed record Order(
    string OrderId,
    string CustomerId,
    DateOnly Date,
    OrderStatus Status)
{
    public List<OrderEntry> Entries { get; init; } = [];
}