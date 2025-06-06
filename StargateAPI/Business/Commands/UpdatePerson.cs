using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Commands;

public class UpdatePerson : IRequest
{
    public required string Name { get; set; }
}

public class UpdatePersonPreProcessor(StargateContext context) : IRequestPreProcessor<UpdatePerson>
{
    public async Task Process(UpdatePerson request, CancellationToken cancellationToken)
    {
        var person = await context
            .People.AsNoTracking()
            .FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);
        if (person is null)
            throw new BadHttpRequestException("Person not found");
    }
}

public class UpdatePersonHandler(StargateContext context) : IRequestHandler<UpdatePerson>
{
    public async Task Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        var person = await context.People.FirstOrDefaultAsync(
            z => z.Name == request.Name,
            cancellationToken
        );
        if (person is null)
            throw new BadHttpRequestException("Person not found");

        person.Name = request.Name;
        await context.SaveChangesAsync(cancellationToken);
    }
}
