
using Domain;

namespace Application.Models;

public sealed record PeriodResult(
    DateOnly From,
    DateOnly To,
    Product Product
);