using Application.Interfaces;

using Domain;

using Infrastructure.Json;

using System.Text.Json;

namespace Infrastructure.Repositories;

public class JsonOrderRepository : IOrderRepository
{
    private readonly string _filePath;

    public JsonOrderRepository()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inputs", "orders.json");
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {

        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"Orders file not found at path: {_filePath}");

        var json = await File.ReadAllTextAsync(_filePath);

        try
        {
            // Return empty list if file exists but contains null/empty JSON
            return JsonSerializer.Deserialize<List<Order>>(json, JsonOptions.Default) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse orders.json: {ex.Message}");
        }
    }
}