
using Application.Models;

namespace Application.Interfaces;

public interface ISizzlingHotService
{
    Task<SizzlingHotResult> GetSizzlingHotAsync(DateOnly from, DateOnly to);
}