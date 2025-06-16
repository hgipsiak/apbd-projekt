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
        var transaction = await _context.Database.BeginTransactionAsync();


        var exists = await _context.Persons.FirstOrDefaultAsync(e => e.IdClient == id);

        if (exists == null)
        {
            throw new NotFoundException("Person not found");
        }

        await _context.Persons.Where(e => e.IdClient == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(e => e.DeletionDate, DateTime.Now));
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}