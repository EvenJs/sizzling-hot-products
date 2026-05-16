using Application.Interfaces;
using Application.Services;

using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// DI Registration
builder.Services.AddSingleton<IOrderRepository, JsonOrderRepository>();
builder.Services.AddSingleton<IProductRepository, JsonProductRepository>();
builder.Services.AddScoped<ISizzlingHotService, SizzlingHotService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});
app.UseCors("Frontend");

app.MapGet("/api/products/sizzling-hot/daily", async (ISizzlingHotService service) =>
{
    var from = new DateOnly(2026, 4, 21);
    var to = new DateOnly(2026, 4, 23);

    var result = await service.GetDailyTopProductsAsync(from, to);
    return Results.Ok(result);
});

app.MapGet("/api/products/sizzling-hot/period", async (ISizzlingHotService service) =>
{
    var from = new DateOnly(2026, 4, 21);
    var to = new DateOnly(2026, 4, 23);

    var result = await service.GetPeriodTopProductAsync(from, to);
    return Results.Ok(result);
});

app.Run();
