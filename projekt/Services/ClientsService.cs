﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;

namespace projekt.Services;

public class ClientsService : IClientsService
{
    private readonly DatabaseContext _context;

    public ClientsService(DatabaseContext context)
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

        if (!int.TryParse(dto.PhoneNumber, out int number) || !long.TryParse(dto.Pesel, out long pesel))
        {
            throw new BadRequestException("Phone number or pesel contains invalid characters");
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
            var exists = await _context.Persons.FirstOrDefaultAsync(e => e.IdClient == id);

            if (exists == null)
            {
                throw new NotFoundException("Person not found");
            }

            if (dto.PhoneNumber.Length != 9)
            {
                throw new BadRequestException("Phone number must be 9 digits");
            }
            
            if (!int.TryParse(dto.PhoneNumber, out int number))
            {
                throw new BadRequestException("Phone number contains invalid characters");
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
            var exists = await _context.Companies.FirstOrDefaultAsync(e => e.IdClient == id);

            if (exists == null)
            {
                throw new NotFoundException("Company not found");
            }

            if (dto.PhoneNumber.Length != 9)
            {
                throw new BadRequestException("Phone number must be 9 digits");
            }

            if (!int.TryParse(dto.PhoneNumber, out int number))
            {
                throw new BadRequestException("Phone number contains invalid characters");
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
}