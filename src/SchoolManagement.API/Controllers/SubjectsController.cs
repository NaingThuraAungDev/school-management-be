using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.ClassManagement.Commands;
using SchoolManagement.Application.Features.ClassManagement.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects()
    {
        var result = await _mediator.Send(new GetSubjectsQuery());
        return Ok(result.Data);
    }

    [HttpPost("teacher-mappings")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> MapSubjectTeacher([FromBody] MapSubjectTeacherCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("teacher-mappings")]
    public async Task<IActionResult> GetSubjectMappings([FromQuery] Guid? classSectionId, [FromQuery] Guid? academicYearId)
    {
        var result = await _mediator.Send(new GetSubjectMappingsQuery(classSectionId, academicYearId));
        return Ok(result.Data);
    }
}
