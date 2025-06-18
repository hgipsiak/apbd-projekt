using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;

namespace projekt.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddNewPerson(PersonClientDto dto)
    {
        var exists = await _context.Persons.FirstOrDefaultAsync(e => e.Pesel == dto.Pesel);

        if (exists != null)
        {
            throw new ConflictException("Person already exists");
        }

        if (dto.PhoneNumber.Length != 9)
        {
            throw new BadRequestException("Phone number must be 9 digits");
        }

        if (dto.Pesel.Length != 11)
        {
            throw new BadRequestException("Pesel must be 11 digits");
        }


        int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int suma = 0;

        for (int i = 0; i < wagi.Length; i++)
        {
            suma += (dto.Pesel[i] - '0') * wagi[i];
        }

        int controlSum = (10 - (suma % 10)) % 10;
        if (controlSum != (dto.Pesel[10] - '0'))
        {
            throw new BadRequestException("Control sum is invalid");
        }

        var person = new Person()
        {
            Pesel = dto.Pesel,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber
        };

        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePerson(int id, PersonClientDto dto)
    {
        var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var exists = await _context.Persons.FirstOrDefaultAsync(e => e.Pesel == dto.Pesel);

            if (exists.IdClient != id)
            {
                throw new NotFoundException("Person not found");
            }

            if (dto.PhoneNumber.Length != 9)
            {
                throw new BadRequestException("Phone number must be 9 digits");
            }
            
            if (!int.TryParse(dto.PhoneNumber, out int number))
            {
                throw new BadRequestException("Phone number contains onvalid characters");
            }

            await _context.Clients.Where(e => e.IdClient == id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.Address, dto.Address)
                        .SetProperty(e => e.Email, dto.Email)
                        .SetProperty(e => e.PhoneNumber, dto.PhoneNumber));

            await _context.SaveChangesAsync();

            await _context.Persons.Where(e => e.IdClient == id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.FirstName, dto.FirstName)
                        .SetProperty(e => e.LastName, dto.LastName));

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletePerson(int id)
    {
        var exists = await _context.Persons.FirstOrDefaultAsync(e => e.IdClient == id);

        if (exists == null)
        {
            throw new NotFoundException("Person not found");
        }

        await _context.Persons.Where(e => e.IdClient == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(e => e.DeletionDate, DateTime.Now));
        await _context.SaveChangesAsync();
    }

    public async Task AddNewCompany(CompanyClientDto dto)
    {
        var exists = await _context.Companies.FirstOrDefaultAsync(e => e.Krs == dto.Krs);

        if (exists != null)
        {
            throw new ConflictException("Company already exists");
        }
        
        if (dto.PhoneNumber.Length != 9)
        {
            throw new BadRequestException("Phone number must be 9 digits");
        }

        if (dto.Krs.Length != 10)
        {
            throw new BadRequestException("Krs must be 10 digits");
        }
        
        if (!int.TryParse(dto.PhoneNumber, out int number) || !long.TryParse(dto.Krs, out long krs))
        {
            throw new BadRequestException("Phone number or krs contains invalid characters");
        }

        var company = new Company()
        {
            Address = dto.Address,
            CompanyName = dto.CompanyName,
            Email = dto.Email,
            Krs = dto.Krs,
            PhoneNumber = dto.PhoneNumber
        };
        
        await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCompany(int id, CompanyClientDto dto)
    {
        var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var exists = await _context.Companies.FirstOrDefaultAsync(e => e.Krs == dto.Krs);

            if (exists.IdClient != id)
            {
                throw new NotFoundException("Company not found");
            }

            if (dto.PhoneNumber.Length != 9)
            {
                throw new BadRequestException("Phone number must be 9 digits");
            }

            if (!int.TryParse(dto.PhoneNumber, out int number))
            {
                throw new BadRequestException("Phone number contains onvalid characters");
            }

            await _context.Clients.Where(e => e.IdClient == id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.Address, dto.Address)
                        .SetProperty(e => e.Email, dto.Email)
                        .SetProperty(e => e.PhoneNumber, dto.PhoneNumber));

            await _context.SaveChangesAsync();

            await _context.Companies.Where(e => e.IdClient == id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.CompanyName, dto.CompanyName));
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CreatePayment(int clientId, int softwareId, PaymentDto dto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(e => e.IdClient == clientId);
        if (client == null)
        {
            throw new NotFoundException("Client not found");
        }
        var software = await _context.Softwares.FirstOrDefaultAsync(e => e.SoftwareId == softwareId
        && e.Version == dto.SoftwareVersion);
        if (software == null)
        {
            throw new NotFoundException("Software not found");
        }
        var exists = await _context.Contracts
            .FirstOrDefaultAsync(e => e.ClientId == clientId && e.SoftwareId == softwareId && e.SoftwareId == softwareId);
        var returningClient = false;
        if (exists != null && exists.EndDate > DateTime.Now)
        {
            throw new ConflictException("Payment on this software already exists");
        } else if (exists != null)
        {
            returningClient = true;
        }
        
        if ((dto.EndDate - dto.StartDate).Days < 3 || (dto.EndDate - dto.StartDate).Days > 30)
        {
            throw new BadRequestException("Difference between start and end dates must be between 3 and 30 days");
        }

        var discounts = await _context.Discounts.Where(e => e.DiscountId == dto.SoftwareId
        && dto.StartDate < e.ToDate).ToListAsync();
        var maxDiscount = discounts.Max(e => e.Value);

        if (returningClient)
        {
            maxDiscount += 0.05m;
        }

        if (dto.UpdateYears < 1 && dto.UpdateYears > 4)
        {
            throw new BadRequestException("UpdateYears must be between 1 and 4 years");
        }

        var discountPrice = (software.Price + (dto.UpdateYears - 1) * 1000) - (software.Price + (dto.UpdateYears - 1) * 1000) * maxDiscount;

        var payment = new Contract()
        {
            ClientId = clientId,
            SoftwareId = softwareId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalPrice = discountPrice,
            UpdateYears = dto.UpdateYears,
            SoftwareVersion = software.Version,
            IsInstalment = false
        };

        if (dto.IsInstalment)
        {
            payment.IsInstalment = true;
            payment.InstalmentsQuantity = dto.InstalmentNumber;
        }
        
        await _context.Contracts.AddAsync(payment);
        await _context.SaveChangesAsync();
    }
}