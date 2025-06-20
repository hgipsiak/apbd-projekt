using projekt.DTOs;

namespace projekt.Services;

public interface IProfitsService
{
    Task<GetProfitDto> CalculateProfit(string currencyCode, int? softwareId = null);
}