using GolbonWebRoad.Application.Features.Logs.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] GetLogsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
