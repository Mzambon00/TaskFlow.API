using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Interfaces;

public interface ITaskService
{
    Task<PaginationResponse<TaskResponse>> GetAllAsync(int pageNumber, int pageSize, Guid userId, bool isAdmin);
    Task<TaskResponse> GetByIdAsync(Guid id, Guid userId, bool isAdmin);
    Task<TaskResponse> CreateAsync(TaskRequest request, Guid userId);
    Task<TaskResponse> UpdateAsync(Guid id, TaskRequest request, Guid userId, bool isAdmin);
    Task DeleteAsync(Guid id, Guid userId, bool isAdmin);
    Task CompleteAsync(Guid id, Guid userId, bool isAdmin);
    Task RestoreAsync(Guid id, Guid userId, bool isAdmin);
}
