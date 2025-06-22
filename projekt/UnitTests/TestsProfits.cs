using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using projekt.Data;
using projekt.Exceptions;
using projekt.Services;

namespace UnitTests;

public class TestsProfits
{
    private readonly DbContextOptions<DatabaseContext> _options;

    public TestsProfits()
    {
        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestProfits")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Fact]
    public async Task CalculateProfits_Should_Throw_NotFoundException_For_Invalid_Request()
    {
        var client = new HttpClient();
        var context = new DatabaseContext(_options);
        var service = new ProfitsService(context, client);
        
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.CalculateProfit("abc"));
        Assert.Equal("Invalid request", ex.Message);
    }
}