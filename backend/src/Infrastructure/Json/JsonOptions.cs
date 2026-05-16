using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Json;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}