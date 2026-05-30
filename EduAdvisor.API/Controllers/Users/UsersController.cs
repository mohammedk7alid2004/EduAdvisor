using EduAdvisor.Application.Queries.Users;
namespace EduAdvisor.API.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("{userId}/roles")]
        [HasPermission(Permissions.RolesUpdate)] 
        public async Task<IActionResult> AssignRoles(string userId, [FromBody] List<string> roles)
        {
            var command = new AssignRolesToUserCommand(userId, roles);
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{userId}/roles")]
        [HasPermission(Permissions.RolesRead)] 
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var result = await _mediator.Send(new GetUserRolesQuery(userId));
            return StatusCode(result.StatusCode, result);
        }
    }
}
