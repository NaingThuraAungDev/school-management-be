using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Staff.Commands;
using SchoolManagement.Application.Features.Staff.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IMediator _mediator;

    public StaffController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Onboard([FromBody] OnboardStaffCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStaffCommand command)
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
        var result = await _mediator.Send(new DeleteStaffCommand(id));
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetStaffByIdQuery(id));
        if (!result.Succeeded)
            return NotFound(new { Errors = result.Errors });
        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetStaffListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpPost("{staffId:guid}/roles")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> AssignRole(Guid staffId, [FromBody] AssignStaffRoleCommand command)
    {
        if (staffId != command.StaffId)
            return BadRequest(new { Error = "Staff ID mismatch" });
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { RoleId = result.Data });
    }
}
