using System.Net;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;

namespace projekt.Services;

public class ProfitsService : IProfitsService
{
    private readonly DatabaseContext _context;
    private readonly HttpClient _httpClient;

    public ProfitsService(DatabaseContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }
    
    public async Task<GetProfitDto> CalculateProfit(string currencyCode, int? softwareId = null)
    {
        List<Contract> contracts;
        if (softwareId == null)
        {
            contracts = await _context.Contracts.Where(e => e.IsFulfilled)
                .Include(e => e.Software).ToListAsync();
        }
        else
        {
            contracts = await _context.Contracts.Where(e => e.IsFulfilled && e.SoftwareId == softwareId)
                .Include(e => e.Software).ToListAsync();
        }
        var grouped = contracts.GroupBy(e => e.Software).ToList();
        var res = new GetProfitDto()
        {
            CurrencyCode = currencyCode,
            Sum = contracts.Sum(e => e.TotalPrice),
            Softwares = grouped.Select(e => new GetSoftwareDto()
            {
                SoftwareId = e.Key.SoftwareId,
                SoftwareName = e.Key.Name,
                Contracts = e.Select(c => new GetContractDto()
                {
                    ContractId = c.ContractId,
                    TotalPrice = c.TotalPrice,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                }).ToList()
            }).ToList()
        };
        if (currencyCode.ToUpper() == "PLN") return res;

        var url = $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/?format=json";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException("Invalid request");
        }

        var rateItem = response.Content.ReadFromJsonAsync<GetNBPDto>().Result;
        var rate = rateItem.Rates.FirstOrDefault().Mid;

        res.Sum /= rate;
        res.Sum = decimal.Round(res.Sum, 2);
        foreach (var software in res.Softwares)
        {
            foreach (var contract in software.Contracts)
            {
                contract.TotalPrice /= rate;
                contract.TotalPrice = decimal.Round(contract.TotalPrice, 2);
            }
        }
        return res;
    }
}