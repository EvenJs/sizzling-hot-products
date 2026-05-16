

using Application.Interfaces;
using Application.Models;

using Domain;

namespace Application.Services;


public class SizzlingHotService : ISizzlingHotService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public SizzlingHotService(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<DailyResult>> GetDailyTopProductsAsync(DateOnly from, DateOnly to)
    {
        var orders = await _orderRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();

        var validOrders = ResolveCancelledOrders(orders);
        var sales = GetDeduplicatedSales(validOrders);

        var results = new List<DailyResult>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var dailySales = sales.Where(s => s.Date == date);
            var topProduct = GetTopProduct(dailySales, products);

            if (topProduct != null)
            {
                results.Add(new DailyResult(date, topProduct));
            }
        }
        return results;
    }

    public async Task<PeriodResult?> GetPeriodTopProductAsync(DateOnly from, DateOnly to)
    {
        var orders = await _orderRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();

        var validOrders = ResolveCancelledOrders(orders);
        // Date range is inclusive on both ends
        var sales = GetDeduplicatedSales(validOrders)
            .Where(s => s.Date >= from && s.Date <= to);

        var topProduct = GetTopProduct(sales, products);

        return topProduct != null ? new PeriodResult(from, to, topProduct) : null;
    }

    private static IEnumerable<Order> ResolveCancelledOrders(IEnumerable<Order> orders)
    {
        // Use HashSet for O(1) lookup performance when filtering cancelled orders
        var cancelledIds = orders.Where(t => t.Status == OrderStatus.Cancelled)
            .Select(t => t.OrderId).ToHashSet();

        return orders.Where(t => t.Status == OrderStatus.Completed && !cancelledIds.Contains(t.OrderId));
    }

    // Distinct on the tuple satisfies both rule 1 (quantity ignored) and
    // rule 2 (same customer, same product, same day = one sale)
    private static IEnumerable<(string ProductId, string CustomerId, DateOnly Date)> GetDeduplicatedSales(IEnumerable<Order> orders)
    {
        return orders.
            SelectMany(order => order.Entries.Select(product => (
                ProductId: product.Id,
                CustomerId: order.CustomerId,
                Date: order.Date
            ))).Distinct();
    }

    // ThenBy name ascending satisfies rule 4 — alphabetical tie-breaking
    private static Product? GetTopProduct(IEnumerable<(string ProductId, string CustomerId, DateOnly Date)> sales, IEnumerable<Product> products)
    {
        return sales.GroupBy(s => s.ProductId)
        .Select(g => new
        {
            Product = products.FirstOrDefault(p => p.Id == g.Key),
            Count = g.Count()
        })
        .Where(x => x.Product != null)
        .OrderByDescending(x => x.Count)
        .ThenBy(x => x.Product!.Name)
        .Select(x => x.Product)
        .FirstOrDefault();
    }
}