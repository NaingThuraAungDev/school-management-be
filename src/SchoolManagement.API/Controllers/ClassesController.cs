using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.ClassManagement.Commands;
using SchoolManagement.Application.Features.ClassManagement.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClassesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- Classes ---

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetClasses()
    {
        var result = await _mediator.Send(new GetClassesQuery());
        return Ok(result.Data);
    }

    // --- Sections ---

    [HttpPost("sections")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateSection([FromBody] CreateSectionCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("sections")]
    public async Task<IActionResult> GetSections()
    {
        var result = await _mediator.Send(new GetSectionsQuery());
        return Ok(result.Data);
    }

    // --- Class-Sections ---

    [HttpPost("class-sections")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateClassSection([FromBody] CreateClassSectionCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("class-sections")]
    public async Task<IActionResult> GetClassSections([FromQuery] Guid? classId)
    {
        var result = await _mediator.Send(new GetClassSectionsQuery { ClassId = classId });
        return Ok(result.Data);
    }
}
