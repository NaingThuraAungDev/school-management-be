using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Students.Commands;
using SchoolManagement.Application.Features.Students.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Admit([FromBody] AdmitStudentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Error = "ID mismatch" });
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteStudentCommand(id));
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetStudentByIdQuery(id));
        if (!result.Succeeded)
            return NotFound(new { Errors = result.Errors });
        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetStudentsListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpPost("{studentId:guid}/guardians")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> LinkGuardian(Guid studentId, [FromBody] LinkGuardianCommand command)
    {
        if (studentId != command.StudentId)
            return BadRequest(new { Error = "Student ID mismatch" });
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { GuardianId = result.Data });
    }

    [HttpPost("{studentId:guid}/documents")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> UploadDocument(Guid studentId, [FromForm] UploadDocumentCommand command)
    {
        if (studentId != command.StudentId)
            return BadRequest(new { Error = "Student ID mismatch" });
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { DocumentId = result.Data });
    }
}
