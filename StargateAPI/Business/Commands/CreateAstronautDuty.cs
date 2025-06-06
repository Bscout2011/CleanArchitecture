using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Commands;

public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
{
    public required string Name { get; set; }

    public required string Rank { get; set; }

    public required string DutyTitle { get; set; }

    public DateTime DutyStartDate { get; set; }
}

public class CreateAstronautDutyPreProcessor(StargateContext context)
    : IRequestPreProcessor<CreateAstronautDuty>
{
    public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
    {
        var person = await context
            .People.AsNoTracking()
            .FirstOrDefaultAsync(z => z.Name == request.Name);

        if (person is null)
        {
            throw new BadHttpRequestException("New Astronaut Duty must be related to a Person.");
        }

        var verifyNoPreviousDuty = await context.AstronautDuties.FirstOrDefaultAsync(
            z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate
        );

        if (verifyNoPreviousDuty is not null)
        {
            throw new BadHttpRequestException(
                $"Astronaut Duty already exists: Title = {request.DutyTitle}, Start Date = {request.DutyStartDate}"
            );
        }

        return;
    }
}

public class CreateAstronautDutyHandler(StargateContext context)
    : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
{
    public async Task<CreateAstronautDutyResult> Handle(
        CreateAstronautDuty request,
        CancellationToken cancellationToken
    )
    {
        var person =
            await context
                .People.Include(p => p.AstronautDetail)
                .Include(p => p.AstronautDuties)
                .AsSplitQuery()
                .FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken)
            ?? throw new BadHttpRequestException($"Person with name {request.Name} not found.");

        if (person.AstronautDetail == null)
        {
            person.AstronautDetail = new()
            {
                PersonId = person.Id,
                Person = person,
                CurrentDutyTitle = request.DutyTitle,
                CurrentRank = request.Rank,
                CareerStartDate = request.DutyStartDate.Date,
                CareerEndDate = request.DutyTitle == "RETIRED" ? request.DutyStartDate.Date : null
            };
        }
        else
        {
            person.AstronautDetail.CurrentDutyTitle = request.DutyTitle;
            person.AstronautDetail.CurrentRank = request.Rank;
            if (request.DutyTitle == "RETIRED")
            {
                person.AstronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
            }
        }

        var astronautDuty = person
            .AstronautDuties.OrderByDescending(x => x.DutyStartDate)
            .FirstOrDefault();

        if (astronautDuty != null)
        {
            astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
        }
        var newAstronautDuty = new AstronautDuty()
        {
            PersonId = person.Id,
            Person = person,
            Rank = request.Rank,
            DutyTitle = request.DutyTitle,
            DutyStartDate = request.DutyStartDate.Date,
            DutyEndDate = null
        };
        person.AstronautDuties.Add(newAstronautDuty);

        await context.SaveChangesAsync(cancellationToken);

        return new CreateAstronautDutyResult() { Id = newAstronautDuty.Id };
    }
}

public class CreateAstronautDutyResult
{
    public int Id { get; set; }
}
