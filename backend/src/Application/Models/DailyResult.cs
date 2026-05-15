using Domain;
namespace Application.Models;

public sealed record DailyResult(
    DateOnly Date,
    Product Product);