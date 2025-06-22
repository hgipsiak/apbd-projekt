using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using projekt.Data;
using projekt.DTOs;
using projekt.Exceptions;
using projekt.Models;
using projekt.Services;

namespace UnitTests;

public class TestsContracts
{
    private readonly DbContextOptions<DatabaseContext> _options;

    public TestsContracts()
    {
        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestContracts")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Fact]
    public async Task CreateContract_Should_Throw_NotFoundException_For_Non_Existent_Client()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-06-21"),
            EndDate = DateTime.Parse("2025-07-10"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 2
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.CreateContract(int.MaxValue, int.MaxValue, contract));
        Assert.Equal("Client not found", ex.Message);
    }
    
    [Fact]
    public async Task CreateContract_Should_Throw_NotFoundException_For_Non_Existent_Software()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-06-21"),
            EndDate = DateTime.Parse("2025-07-10"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 2
        };
        
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };

        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.CreateContract(person.IdClient, int.MaxValue, contract));
        Assert.Equal("Software not found", ex.Message);
    }
    
    [Fact]
    public async Task CreateContract_Should_Throw_ConflictException_For_EndDate_Earlier_Than_StartDate()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-07-21"),
            EndDate = DateTime.Parse("2025-06-10"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 2
        };

        var software = new Software()
        {
            Name = "OfficePro",
            Description = "Pakiet biurowy",
            Version = 2025.1m,
            Category = "Biurowe",
            Price = 499.99m
        };

        await context.Softwares.AddAsync(software);
        await context.SaveChangesAsync();
        
        var person = await context.Persons.FirstOrDefaultAsync();

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.CreateContract(person.IdClient, software.SoftwareId, contract));
        Assert.Equal("End date cannot be before start date", ex.Message);
    }
    
    [Fact]
    public async Task CreateContract_Should_Throw_ConflictException_For_Already_Existing_Contract()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-06-21"),
            EndDate = DateTime.Parse("2025-07-10"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 2,
            SoftwareVersion = 2025.1m
        };

        var software = new Software()
        {
            Name = "OfficePro",
            Description = "Pakiet biurowy",
            Version = 2025.1m,
            Category = "Biurowe",
            Price = 499.99m
        };
        
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };

        await context.Softwares.AddAsync(software);
        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();
        

        await service.CreateContract(person.IdClient, software.SoftwareId, contract);
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>  service.CreateContract(person.IdClient, software.SoftwareId, contract));
        Assert.Equal("Contract on this software already exists", ex.Message);
    }
    
    [Fact]
    public async Task CreateContract_Should_Throw_BadRequestException_For_Invalid_Period_Between_StartDate_And_EndDate()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-06-01"),
            EndDate = DateTime.Parse("2025-07-29"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 2,
            SoftwareVersion = 2025.1m
        };

        var software = new Software()
        {
            Name = "OfficePro",
            Description = "Pakiet biurowy",
            Version = 2025.1m,
            Category = "Biurowe",
            Price = 499.99m
        };
        
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };

        await context.Softwares.AddAsync(software);
        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>  service.CreateContract(person.IdClient, software.SoftwareId, contract));
        Assert.Equal("Difference between start and end dates must be between 3 and 30 days", ex.Message);
    }
    
    [Fact]
    public async Task CreateContract_Should_Throw_BadRequestException_For_Invalid_UpdateYears()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var contract = new ContractDto()
        {
            StartDate = DateTime.Parse("2025-06-21"),
            EndDate = DateTime.Parse("2025-07-10"),
            IsInstalment = false,
            InstalmentNumber = null,
            UpdateYears = 6,
            SoftwareVersion = 2025.1m
        };

        var software = new Software()
        {
            Name = "OfficePro",
            Description = "Pakiet biurowy",
            Version = 2025.1m,
            Category = "Biurowe",
            Price = 499.99m
        };
        
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };

        await context.Softwares.AddAsync(software);
        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<BadRequestException>(() =>  service.CreateContract(person.IdClient, software.SoftwareId, contract));
        Assert.Equal("UpdateYears must be between 1 and 4 years", ex.Message);
    }

    [Fact]
    public async Task PayContract_Should_Throw_NotFoundException_For_Non_Existing_Contract()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.PayContract(int.MaxValue));
        Assert.Equal("Contract not found", ex.Message);
    }

    [Fact]
    public async Task PayContract_Should_Throw_BadRequestException_For_Already_Paid_Contract()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);

        var software = new Software()
        {
            Name = "OfficePro",
            Description = "Pakiet biurowy",
            Version = 2025.1m,
            Category = "Biurowe",
            Price = 499.99m
        };
        
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St",
            Email = "john.doe@example.pl",
            Pesel = "92030412346",
            PhoneNumber = "123456789"
        };

        await context.Softwares.AddAsync(software);
        await context.Persons.AddAsync(person);
        await context.SaveChangesAsync();
        
        var contract = new Contract()
        {
            StartDate = DateTime.Parse("2025-06-21"),
            EndDate = DateTime.Parse("2025-07-10"),
            IsInstalment = true,
            UpdateYears = 6,
            SoftwareVersion = 2025.1m,
            ClientId = person.IdClient,
            SoftwareId = software.SoftwareId,
            IsFulfilled = true,
            InstalmentsQuantity = 5,
            TotalPrice = 2500
        };
        
        await context.Contracts.AddAsync(contract);
        await context.SaveChangesAsync();

        for (int i = 0; i < 5; i++)
        {
            var payment = new Payment()
            {
                ContractId = contract.ContractId,
                PaymentDate = DateTime.Parse("2025-05-21"),
                Price = 500
            };
            
            await context.Payments.AddAsync(payment);
        }
        
        await context.SaveChangesAsync();
        
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>  service.PayContract(contract.ContractId));
        Assert.Equal("Contract has been already paid", ex.Message);
    }

    [Fact]
    public async Task DeleteContract_Should_Throw_NotFoundException_For_Non_Existing_Contract()
    {
        var context = new DatabaseContext(_options);
        var service = new ContractsService(context);
        
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteContract(int.MaxValue));
        Assert.Equal("Contract not found", ex.Message);
    }
}