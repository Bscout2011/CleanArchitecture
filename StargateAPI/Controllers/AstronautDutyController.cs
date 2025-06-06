using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[Controller]")]
public class AstronautDutyController(IMediator mediator) : ControllerBase
{
    [HttpGet("{name}")]
    public async Task<IActionResult> GetAstronautDutiesByName(string name)
    {
        var result = await mediator.Send(new GetAstronautDutiesByName() { Name = name });

        if (result is null)
        {
            return NotFound();
        }
        return Ok(result.AstronautDuties);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
    {
        var result = await mediator.Send(request);
        return Ok(result.Id);
    }
}
