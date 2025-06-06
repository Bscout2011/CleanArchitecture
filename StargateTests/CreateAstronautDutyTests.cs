using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

namespace StargateTests;

public class CreateAstronautDutyTests
{
    [Fact]
    public async Task CreateAstronautDutyHandler_CreatesDutyAndUpdatesAstronautDetail()
    {
        // Arrange
        var context = StargateTestDb.GetInMemoryContext();
        var person = new Person { Name = "Teal'c" };
        context.People.Add(person);
        await context.SaveChangesAsync();

        var handler = new CreateAstronautDutyHandler(context);
        var request = new CreateAstronautDuty
        {
            Name = "Teal'c",
            Rank = "Jaffa",
            DutyTitle = "First Prime",
            DutyStartDate = new DateTime(2000, 1, 1)
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotEqual(0, result.Id);
        var duty = await context.AstronautDuties.FindAsync(result.Id);
        Assert.NotNull(duty);
        Assert.Equal("Jaffa", duty!.Rank);
        Assert.Equal("First Prime", duty.DutyTitle);
        Assert.Equal(new DateTime(2000, 1, 1), duty.DutyStartDate);

        var updatedPerson = await context
            .People.Include(p => p.AstronautDetail)
            .FirstOrDefaultAsync(p => p.Id == person.Id);
        Assert.NotNull(updatedPerson!.AstronautDetail);
        Assert.Equal("Jaffa", updatedPerson.AstronautDetail!.CurrentRank);
        Assert.Equal("First Prime", updatedPerson.AstronautDetail.CurrentDutyTitle);
    }

    [Fact]
    public async Task CreateAstronautDutyPreProcessor_ThrowsIfPersonNotFound()
    {
        // Arrange
        var context = StargateTestDb.GetInMemoryContext();
        var preProcessor = new CreateAstronautDutyPreProcessor(context);
        var request = new CreateAstronautDuty
        {
            Name = "Jonas Quinn",
            Rank = "Scientist",
            DutyTitle = "Research",
            DutyStartDate = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadHttpRequestException>(
            () => preProcessor.Process(request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateAstronautDutyPreProcessor_ThrowsIfDutyAlreadyExists()
    {
        // Arrange
        var context = StargateTestDb.GetInMemoryContext();
        var person = new Person { Name = "Vala Mal Doran" };
        context.People.Add(person);
        context.AstronautDuties.Add(
            new AstronautDuty
            {
                Person = person,
                PersonId = person.Id,
                Rank = "Civilian",
                DutyTitle = "Negotiator",
                DutyStartDate = new DateTime(2010, 5, 1)
            }
        );
        await context.SaveChangesAsync();

        var preProcessor = new CreateAstronautDutyPreProcessor(context);
        var request = new CreateAstronautDuty
        {
            Name = "Vala Mal Doran",
            Rank = "Civilian",
            DutyTitle = "Negotiator",
            DutyStartDate = new DateTime(2010, 5, 1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadHttpRequestException>(
            () => preProcessor.Process(request, CancellationToken.None)
        );
    }
}
