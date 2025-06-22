using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;
using projekt.Services;

namespace UnitTests;

public class TestsClients
{
    private readonly DbContextOptions<DatabaseContext> _options;

    public TestsClients()
    {
        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestClients")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }
    
    [Fact]
    public async Task AddNewPerson_Should_Throw_ConflictException_For_Already_Added_Person()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };
        
        await clientsService.AddNewPerson(person);
        var ex = await Assert.ThrowsAsync<ConflictException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Person already exists", ex.Message);
    }
    
    [Fact]
    public async Task AddNewPerson_Should_Throw_ConflictException_For_Person_With_Invalid_PhoneNumber_Incorrect_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "62093044357",
            PhoneNumber = "12345678"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Phone number must be 9 digits", ex.Message);
    }

    [Fact]
    public async Task AddNewPerson_Should_Throw_ConflictException_For_Person_With_Invalid_PhoneNumber()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "62093044357",
            PhoneNumber = "12345678a"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Phone number or pesel contains invalid characters", ex.Message);
    }

    [Fact]
    public async Task AddNewPerson_Should_Throw_BadRequestException_For_Person_With_Invalid_Pesel_Incorrect_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "6209304435",
            PhoneNumber = "123456789"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Pesel must be 11 digits", ex.Message);
    }
    
    [Fact]
    public async Task AddNewPerson_Should_Throw_BadRequestException_For_Person_With_Invalid_Pesel_Invalid_Characters()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "6209304435a",
            PhoneNumber = "123456789"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Phone number or pesel contains invalid characters", ex.Message);
    }
    
    [Fact]
    public async Task AddNewPerson_Should_Throw_BadRequestException_For_Person_With_Invalid_Pesel_Invalid_Control_Sum()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "62093044351",
            PhoneNumber = "123456789"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewPerson(person));
        Assert.Equal("Control sum is invalid", ex.Message);
    }

    [Fact]
    public async Task UpdatePerson_Should_Throw_NotFoundException_For_Person_NotFound()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "Jane",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "01120178916",
            PhoneNumber = "123456789"
        };
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => clientsService.UpdatePerson(int.MaxValue, person));
        Assert.Equal("Person not found", ex.Message);
    }

    [Fact]
    public async Task UpdatePerson_Should_Throw_BadRequestException_For_Person_With_Invalid_PhoneNumber_Invalid_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "Jane",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "01120178916",
            PhoneNumber = "12345678"
        };
        
        var existingPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "01120178916",
            PhoneNumber = "123456789"
        };

        await context.AddAsync(existingPerson);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.UpdatePerson(existingPerson.IdClient, person));
        Assert.Equal("Phone number must be 9 digits", ex.Message);
    }
    
    [Fact]
    public async Task UpdatePerson_Should_Throw_BadRequestException_For_Person_With_Invalid_PhoneNumber()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var person = new PersonClientDto()
        {
            FirstName = "Jane",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "01120178916",
            PhoneNumber = "12345678a"
        };
        
        var existingPerson = new Person
        {
            FirstName = "Jane",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "01120178916",
            PhoneNumber = "123456789"
        };

        await context.AddAsync(existingPerson);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.UpdatePerson(existingPerson.IdClient, person));
        Assert.Equal("Phone number contains invalid characters", ex.Message);
    }
    
    [Fact]
    public async Task DeletePerson_Should_Throw_NotFoundException_For_Person_NotFound()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => clientsService.DeletePerson(int.MaxValue));
        Assert.Equal("Person not found", ex.Message);
    }
    
    [Fact]
    public async Task AddNewCompany_Should_Throw_ConflictException_For_Already_Added_Company()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "1234567890",
            PhoneNumber = "987654321"
        };
        
        await clientsService.AddNewCompany(company);
        var ex = await Assert.ThrowsAsync<ConflictException>(() => clientsService.AddNewCompany(company));
        Assert.Equal("Company already exists", ex.Message);
    }
    
    [Fact]
    public async Task AddNewCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_PhoneNumber_Invalid_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987654321",
            PhoneNumber = "98765432"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewCompany(company));
        Assert.Equal("Phone number must be 9 digits", ex.Message);
    }
    
    [Fact]
    public async Task AddNewCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_PhoneNumber()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987654321",
            PhoneNumber = "98765432a"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewCompany(company));
        Assert.Equal("Phone number or krs contains invalid characters", ex.Message);
    }
    
    [Fact]
    public async Task AddNewCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_Krs_Invalid_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "098765432",
            PhoneNumber = "987654321"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewCompany(company));
        Assert.Equal("Krs must be 10 digits", ex.Message);
    }
    
    [Fact]
    public async Task AddNewCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_Krs()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "098765432a",
            PhoneNumber = "987654321"
        };
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.AddNewCompany(company));
        Assert.Equal("Phone number or krs contains invalid characters", ex.Message);
    }
    
    [Fact]
    public async Task UpdateCompany_Should_Throw_NotFoundException_For_Company_NotFound()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987654321",
            PhoneNumber = "98765432"
        };
        
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => clientsService.UpdateCompany(int.MaxValue, company));
        Assert.Equal("Company not found", ex.Message);
    }
    
    [Fact]
    public async Task UpdateCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_PhoneNumber_Invalid_Length()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987564321",
            PhoneNumber = "98765432"
        };
        
        var companyExists = new Company()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987564321",
            PhoneNumber = "987654321"
        };
        
        await context.AddAsync(companyExists);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.UpdateCompany(companyExists.IdClient, company));
        Assert.Equal("Phone number must be 9 digits",  ex.Message);
    }
    
    [Fact]
    public async Task UpdateCompany_Should_Throw_BadRequestException_For_Company_With_Invalid_PhoneNumber()
    {
        var context = new DatabaseContext(_options);
        var clientsService = new ClientsService(context);
        var company = new CompanyClientDto()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987564321",
            PhoneNumber = "98765432a"
        };
        
        var companyExists = new Company()
        {
            CompanyName = "Company",
            Address = "123 Main St",
            Email = "company@example.com",
            Krs = "0987564321",
            PhoneNumber = "987654321"
        };
        
        await context.AddAsync(companyExists);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => clientsService.UpdateCompany(companyExists.IdClient, company));
        Assert.Equal("Phone number contains invalid characters", ex.Message);
    }
}