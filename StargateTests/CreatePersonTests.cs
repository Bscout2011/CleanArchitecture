using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory; // Added namespace for InMemory database support
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using Xunit;

namespace StargateTests;

public class CreatePersonTests
{
    private StargateContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<StargateContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new StargateContext(options);
    }

    [Fact]
    public async Task CreatePersonHandler_CreatesPerson()
    {
        // Arrange
        var context = GetInMemoryContext();
        var handler = new CreatePersonHandler(context);
        var request = new CreatePerson { Name = "Jack O'Neill" };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(0, result.Id);
        var person = await context.People.FindAsync(result.Id);
        Assert.NotNull(person);
        Assert.Equal("Jack O'Neill", person!.Name);
    }

    [Fact]
    public async Task CreatePersonPreProcessor_ThrowsIfNameExists()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.People.Add(new Person { Name = "Samantha Carter" });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var preProcessor = new CreatePersonPreProcessor(context);
        var request = new CreatePerson { Name = "Samantha Carter" };

        // Act & Assert
        await Assert.ThrowsAsync<BadHttpRequestException>(
            () => preProcessor.Process(request, CancellationToken.None)
        );
    }
}
