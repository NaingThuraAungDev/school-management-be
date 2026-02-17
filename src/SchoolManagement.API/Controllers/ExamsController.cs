using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Exams.Commands;
using SchoolManagement.Application.Features.Exams.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- Exam Terms ---

    [HttpPost("terms")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateExamTerm([FromBody] CreateExamTermCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("terms")]
    public async Task<IActionResult> GetExamTerms([FromQuery] Guid academicYearId)
    {
        var result = await _mediator.Send(new GetExamTermsQuery(academicYearId));
        return Ok(result.Data);
    }

    // --- Grade Definitions ---

    [HttpPost("grades")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateGradeDefinition([FromBody] CreateGradeDefinitionCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("grades")]
    public async Task<IActionResult> GetGradeDefinitions([FromQuery] Guid academicYearId)
    {
        var result = await _mediator.Send(new GetGradeDefinitionsQuery(academicYearId));
        return Ok(result.Data);
    }

    // --- Exams ---

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> CreateExam([FromBody] CreateExamCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetExams([FromQuery] GetExamsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    // --- Results ---

    [HttpPost("results")]
    [Authorize(Roles = "SuperAdmin,Admin,Teacher")]
    public async Task<IActionResult> RecordResult([FromBody] RecordExamResultCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(result.Data);
    }

    [HttpGet("results")]
    public async Task<IActionResult> GetResults([FromQuery] GetExamResultsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    // --- Report Cards ---

    [HttpPost("report-card-templates")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> CreateReportCardTemplate([FromBody] CreateReportCardTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { Id = result.Data });
    }

    [HttpGet("report-card/{studentId:guid}")]
    public async Task<IActionResult> GetReportCard(Guid studentId, [FromQuery] Guid examTermId)
    {
        var result = await _mediator.Send(new GetReportCardQuery(studentId, examTermId));
        if (!result.Succeeded)
            return NotFound(new { Errors = result.Errors });
        return Ok(result.Data);
    }
}
