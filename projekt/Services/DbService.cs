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

    public async Task AddNewPerson([FromBody] AddPersonClientDto dto)
    {
        var exists = await _context.Persons.FirstOrDefaultAsync(e => e.Pesel == dto.Pesel);

        if (exists != null)
        {
            throw new ConflictException("Person already exists");
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
        
        _context.Persons.Add(person);
        await _context.SaveChangesAsync();
    }
}