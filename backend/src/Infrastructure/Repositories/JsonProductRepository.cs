using Application.Interfaces;

using Domain;

using Infrastructure.Json;

using System.Text.Json;

namespace Infrastructure.Repositories;

public class JsonProductRepository : IProductRepository
{
    private readonly string _filePath;

    public JsonProductRepository()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inputs", "products.json");
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {

        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"Products file not found at path: {_filePath}");

        var json = await File.ReadAllTextAsync(_filePath);

        try
        {
            // Return empty list if file exists but contains null/empty JSON
            return JsonSerializer.Deserialize<List<Product>>(json, JsonOptions.Default) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse products.json: {ex.Message}");
        }
    }
}