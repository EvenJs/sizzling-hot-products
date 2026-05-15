
using Application.Models;

namespace Application.Interfaces;

public interface ISizzlingHotService
{
    Task<IEnumerable<DailyResult>> GetDailyTopProductsAsync(DateOnly from, DateOnly to);
    Task<PeriodResult?> GetPeriodTopProductAsync(DateOnly from, DateOnly to);
}