
namespace Application.Models;

public sealed record SizzlingHotResult(
    IEnumerable<DailyResult> Daily,
    PeriodResult? Period
);