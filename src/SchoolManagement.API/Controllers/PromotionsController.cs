using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Features.Promotions.Commands;
using SchoolManagement.Application.Features.Promotions.Queries;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class PromotionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromotionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> BulkPromote([FromBody] BulkPromoteStudentsCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
            return BadRequest(new { Errors = result.Errors });
        return Ok(new { PromotedCount = result.Data });
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview([FromQuery] GetPromotionPreviewQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }
}
