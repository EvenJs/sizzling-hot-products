using Microsoft.AspNetCore.Mvc.Testing;

using System.Net;

using FluentAssertions;

namespace Application.Tests;

public class SizzlingHotEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SizzlingHotEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSizzlingHot_Returns200_WithValidDateRange()
    {
        // Act
        var response = await _client.GetAsync("/api/products/sizzling-hot?from=2026-04-21&to=2026-04-23");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetSizzlingHot_Returns400_WhenFromIsAfterTo()
    {
        // Act
        var response = await _client.GetAsync("/api/products/sizzling-hot?from=2026-04-23&to=2026-04-21");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}