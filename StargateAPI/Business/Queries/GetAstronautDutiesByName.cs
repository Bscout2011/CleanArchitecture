using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler(StargateContext context)
        : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult?>
    {
        public async Task<GetAstronautDutiesByNameResult?> Handle(
            GetAstronautDutiesByName request,
            CancellationToken cancellationToken
        )
        {
            var person = await context
                .People.Include(p => p.AstronautDetail)
                .Include(p => p.AstronautDuties)
                .Where(p => p.Name == request.Name)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);
            if (person == null)
            {
                return null;
            }
            return new()
            {
                Person = new()
                {
                    PersonId = person.Id,
                    Name = person.Name,
                    CurrentRank = person.AstronautDetail?.CurrentRank ?? string.Empty,
                    CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                    CareerStartDate = person.AstronautDetail?.CareerStartDate,
                    CareerEndDate = person.AstronautDetail?.CareerEndDate
                },
                AstronautDuties = person
                    .AstronautDuties.Select(
                        x =>
                            new AstronautDutyDto()
                            {
                                Id = x.Id,
                                PersonId = x.PersonId,
                                Name = person.Name,
                                Rank = x.Rank,
                                DutyTitle = x.DutyTitle,
                                DutyStartDate = x.DutyStartDate,
                                DutyEndDate = x.DutyEndDate
                            }
                    )
                    .ToList()
            };
        }
    }

    public class GetAstronautDutiesByNameResult
    {
        public required PersonAstronaut Person { get; set; }
        public List<AstronautDutyDto> AstronautDuties { get; set; } = [];
    }
}
