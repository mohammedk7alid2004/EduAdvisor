using EduAdvisor.Application.Commands.Departments;
using EduAdvisor.Application.Queries.Departments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Departments;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("select-menu")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSelectMenu([FromQuery] Guid? facultyId)
    {
        var result = await mediator.Send(new GetDepartmentsSelectMenuQuery(facultyId));
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllDepartmentsQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetDepartmentByIdQuery(id));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
    {
        var result = await mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentCommand command)
    {
        var result = await mediator.Send(command with { Id = id });
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteDepartmentCommand(id));
        return StatusCode(result.StatusCode, result);
    }
}