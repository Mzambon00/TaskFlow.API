using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _taskService.GetAllAsync(pageNumber, pageSize, userId, isAdmin);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _taskService.GetByIdAsync(id, userId, isAdmin);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskRequest request)
    {
        var userId = GetUserId();
        var result = await _taskService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskRequest request)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _taskService.UpdateAsync(id, request, userId, isAdmin);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        await _taskService.DeleteAsync(id, userId, isAdmin);
        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        await _taskService.CompleteAsync(id, userId, isAdmin);
        return NoContent();
    }

    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        await _taskService.RestoreAsync(id, userId, isAdmin);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }
}
