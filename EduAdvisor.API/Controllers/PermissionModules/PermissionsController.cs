using EduAdvisor.Application.Queries.AuthModules;

namespace EduAdvisor.API.Controllers.PermissionModules
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

            [HttpGet]
            [HasPermission(Permissions.PermissionsRead)]
            public async Task<IActionResult> GetPermissions()
            {
                var result = await _mediator.Send(new GetPermissionsQuery());
                return StatusCode(result.StatusCode, result);
            }
    }
}

