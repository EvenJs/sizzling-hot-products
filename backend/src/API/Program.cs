using Application.Interfaces;
using Application.Models;
using Application.Services;

using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// DI Registration
builder.Services.AddSingleton<IOrderRepository, JsonOrderRepository>();
builder.Services.AddSingleton<IProductRepository, JsonProductRepository>();
builder.Services.AddScoped<ISizzlingHotService, SizzlingHotService>();


var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000";
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
})
.WithName("GetDailyTopProducts")
.WithSummary("Get top sizzling hot product per day")
.WithDescription("Returns the top selling product for each day in the date range based on unique customer sales.")
.WithTags("Products")
.Produces<IEnumerable<DailyResult>>(200);

app.MapGet("/api/products/sizzling-hot/period", async (ISizzlingHotService service) =>
{
    var from = new DateOnly(2026, 4, 21);
    var to = new DateOnly(2026, 4, 23);

    var result = await service.GetPeriodTopProductAsync(from, to);
    return Results.Ok(result);
})
.WithName("GetPeriodTopProduct")
.WithSummary("Get top sizzling hot product over a period")
.WithDescription("Returns the top selling product across the entire date range based on unique customer sales.")
.WithTags("Products")
.Produces<PeriodResult>(200)
.Produces(204);

app.Run();
