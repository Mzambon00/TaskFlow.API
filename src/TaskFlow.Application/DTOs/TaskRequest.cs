using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public class TaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Priority Priority { get; set; }
}
