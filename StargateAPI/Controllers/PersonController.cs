using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers;

[ApiController]
[Route("people")]
public class PersonController(IMediator mediator) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetPeople()
    {
        var result = await mediator.Send(new GetPeople() { });

        return Ok(result.People);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetPersonByName(string name)
    {
        var result = await mediator.Send(new GetPersonByName() { Name = name });

        return Ok(result.Person);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePerson request)
    {
        var result = await mediator.Send(request);

        return Ok(result.Id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePersonName([FromBody] UpdatePerson request)
    {
        await mediator.Send(request);
        return Ok();
    }
}
