using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands;

public class CreatePerson : IRequest<CreatePersonResult>
{
    public required string Name { get; set; } = string.Empty;
}

public class CreatePersonPreProcessor(StargateContext context) : IRequestPreProcessor<CreatePerson>
{
    public Task Process(CreatePerson request, CancellationToken cancellationToken)
    {
        var person = context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

        if (person is not null)
            throw new BadHttpRequestException("Bad Request");

        return Task.CompletedTask;
    }
}

public class CreatePersonHandler(StargateContext context)
    : IRequestHandler<CreatePerson, CreatePersonResult>
{
    public async Task<CreatePersonResult> Handle(
        CreatePerson request,
        CancellationToken cancellationToken
    )
    {
        var newPerson = new Person() { Name = request.Name };

        await context.People.AddAsync(newPerson);

        await context.SaveChangesAsync();

        return new CreatePersonResult() { Id = newPerson.Id };
    }
}

public class CreatePersonResult
{
    public int Id { get; set; }
}
