using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Queries;

public class GetPeople : IRequest<GetPeopleResult> { }

public class GetPeopleHandler(StargateContext context) : IRequestHandler<GetPeople, GetPeopleResult>
{
    public async Task<GetPeopleResult> Handle(
        GetPeople request,
        CancellationToken cancellationToken
    )
    {
        var people = await context
            .People.Include(p => p.AstronautDetail)
            .AsNoTracking()
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
            .ToListAsync(cancellationToken);

        return new() { People = people };
    }
}

public class GetPeopleResult
{
    public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };
}
