using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Timetable.Commands;
using SchoolManagement.Application.Features.Timetable.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimetableController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimetableController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- Time Slots ---

    [HttpPost("time-slots")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateTimeSlot([FromBody] CreateTimeSlotCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("time-slots")]
    public async Task<IActionResult> GetTimeSlots()
    {
        var result = await _mediator.Send(new GetTimeSlotsQuery());
        return Ok(result.Data);
    }

    // --- Timetable Entries ---

    [HttpPost("entries")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateEntry([FromBody] CreateTimetableEntryCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpPut("entries/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> UpdateEntry(Guid id, [FromBody] UpdateTimetableEntryCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Error = "ID mismatch" });
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return NoContent();
    }

    [HttpDelete("entries/{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> DeleteEntry(Guid id)
    {
        var result = await _mediator.Send(new DeleteTimetableEntryCommand(id));
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return NoContent();
    }

    [HttpGet("by-class")]
    public async Task<IActionResult> GetByClass([FromQuery] GetTimetableByClassQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("by-teacher")]
    public async Task<IActionResult> GetByTeacher([FromQuery] GetTeacherTimetableQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }
}
