using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GolbonWebRoad.Api.Controllers
{

    public class ApiBaseController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??=  HttpContext.RequestServices.GetRequiredService<IMediator>();

    }
}
