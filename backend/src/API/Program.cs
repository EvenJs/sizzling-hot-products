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


app.MapGet("/api/products/sizzling-hot", async (
    ISizzlingHotService service,
    string? from,
    string? to) =>
{
    var fromDate = DateOnly.TryParse(from, out var parsedFrom)
     ? parsedFrom
     : new DateOnly(2026, 4, 21);

    var toDate = DateOnly.TryParse(to, out var parsedTo)
        ? parsedTo
        : new DateOnly(2026, 4, 23);

    if (fromDate > toDate)
        return Results.BadRequest("'from' date must be before 'to' date.");
    var result = await service.GetSizzlingHotAsync(fromDate, toDate);
    return Results.Ok(result);

})
.WithName("GetSizzlingHotProducts")
.WithSummary("Get sizzling hot products")
.WithDescription("Returns the top selling product per day and over the selected period.")
.WithTags("Products")
.Produces<SizzlingHotResult>(200)
.Produces(400);

app.Run();

public partial class Program { }