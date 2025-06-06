using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Queries;

public class GetPersonByName : IRequest<GetPersonByNameResult>
{
    public required string Name { get; set; } = string.Empty;
}

public class GetPersonByNameHandler(StargateContext context)
    : IRequestHandler<GetPersonByName, GetPersonByNameResult>
{
    public async Task<GetPersonByNameResult> Handle(
        GetPersonByName request,
        CancellationToken cancellationToken
    )
    {
        var person = await context
            .People.Include(p => p.AstronautDetail)
            .AsNoTracking()
            .Where(x => x.Name == request.Name)
            .Select(
                x =>
                    new PersonAstronaut
                    {
                        PersonId = x.Id,
                        Name = x.Name,
                        CurrentRank = x.AstronautDetail!.CurrentRank,
                        CurrentDutyTitle = x.AstronautDetail.CurrentDutyTitle,
                        CareerStartDate = x.AstronautDetail.CareerStartDate,
                        CareerEndDate = x.AstronautDetail.CareerEndDate
                    }
            )
            .FirstOrDefaultAsync(cancellationToken);

        return new() { Person = person };
    }
}

public class GetPersonByNameResult
{
    public PersonAstronaut? Person { get; set; }
}
