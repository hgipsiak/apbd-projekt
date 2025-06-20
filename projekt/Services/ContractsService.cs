using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;

namespace projekt.Services;

public class ContractsService : IContractsService
{
    private readonly DatabaseContext _context;

    public ContractsService(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task CreateContract(int clientId, int softwareId, ContractDto dto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(e => e.IdClient == clientId);
        if (client == null)
        {
            throw new NotFoundException("Client not found");
        }
        var software = await _context.Softwares.FirstOrDefaultAsync(e => e.SoftwareId == softwareId
        && e.Version == dto.SoftwareVersion);
        if (software == null || software.Version != dto.SoftwareVersion)
        {
            throw new NotFoundException("Software not found");
        }
        if (dto.EndDate < dto.StartDate)
        {
            throw new ConflictException("End date cannot be before start date");
        }
        var exists = await _context.Contracts
            .FirstOrDefaultAsync(e => e.ClientId == clientId && e.SoftwareId == softwareId);
        var returningClient = false;
        if (exists != null && exists.EndDate > DateTime.Now)
        {
            throw new ConflictException("Contract on this software already exists");
        } 
        else if (exists != null)
        {
            returningClient = true;
        }
        
        if ((dto.EndDate - dto.StartDate).Days < 3 || (dto.EndDate - dto.StartDate).Days > 30)
        {
            throw new BadRequestException("Difference between start and end dates must be between 3 and 30 days");
        }

        var discounts = await _context.DiscountSoftwares.Where(ds => ds.SoftwareId == softwareId)
            .Select(ds => ds.Discount)
            .Where(d => d.FromDate <= dto.StartDate)
            .ToListAsync();
        var maxDiscount = discounts.Any() ? discounts.Max(e => e.Value) : 0m;

        if (returningClient)
        {
            maxDiscount += 0.05m;
        }

        if (dto.UpdateYears < 1 || dto.UpdateYears > 4)
        {
            throw new BadRequestException("UpdateYears must be between 1 and 4 years");
        }

        var basePrice = software.Price + (dto.UpdateYears -1)*1000;
        var discountPrice = basePrice * (1-maxDiscount);

        var payment = new Contract()
        {
            ClientId = clientId,
            SoftwareId = softwareId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalPrice = discountPrice,
            UpdateYears = dto.UpdateYears,
            SoftwareVersion = software.Version,
            IsInstalment = false,
            IsFulfilled = false
        };

        if (dto.IsInstalment)
        {
            payment.IsInstalment = true;
            payment.InstalmentsQuantity = dto.InstalmentNumber;
        }
        
        await _context.Contracts.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task PayContract(int contractId)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var contract = await _context.Contracts.FirstOrDefaultAsync(e => e.ContractId == contractId);

            if (contract == null)
            {
                throw new NotFoundException("Contract not found");
            }

            if (contract.EndDate < DateTime.Now)
            {
                await _context.Payments.Where(e => e.ContractId == contractId).ExecuteDeleteAsync();
                await _context.SaveChangesAsync();
                throw new BadRequestException("Contract has expired");
            }

            if (contract.IsInstalment)
            {
                var isAlreadyFulfilled = await _context.Payments.Where(e => e.ContractId == contractId).ToListAsync();
                if (isAlreadyFulfilled.Count == contract.InstalmentsQuantity)
                {
                    throw new ConflictException("Contract has been already paid");
                }
                var instalment = new Instalment()
                {
                    ContractId = contractId,
                    PaymentDate = DateTime.Now,
                    Price = contract.TotalPrice / (contract.InstalmentsQuantity ?? 1),
                    InstalmentNumber = isAlreadyFulfilled.Count + 1
                };
                await _context.Instalments.AddAsync(instalment);
                isAlreadyFulfilled.Add(instalment);
                if (isAlreadyFulfilled.Count == contract.InstalmentsQuantity)
                {
                    await _context.Contracts.Where(e => e.ContractId == contractId)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(
                            e => e.IsFulfilled, true));
                }
            }
            else
            {
                var payment = new Payment()
                {
                    Price = contract.TotalPrice,
                    PaymentDate = DateTime.Now,
                    ContractId = contractId
                };
                
                await _context.Contracts.Where(e => e.ContractId == contractId).ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.IsFulfilled, true));
                await _context.Payments.AddAsync(payment);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteContract(int contractId)
    {
        var contract = await _context.Contracts.FirstOrDefaultAsync(e => e.ContractId == contractId);

        if (contract == null)
        {
            throw new NotFoundException("Contract not found");
        }
        
        await _context.Contracts.Where(e => e.ContractId == contractId).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }
}