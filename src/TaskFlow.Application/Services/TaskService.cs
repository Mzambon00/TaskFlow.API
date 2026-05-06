using TaskFlow.Application.DTOs;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;
using AutoMapper;

namespace TaskFlow.Application.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(IRepository<TaskItem> taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<TaskResponse>> GetAllAsync(int pageNumber, int pageSize, Guid userId, bool isAdmin)
    {
        var allTasks = await _taskRepository.FindAsync(t => !isAdmin ? t.UserId == userId : true);
        var query = allTasks.AsQueryable();
        
        var totalRecords = query.Count();
        var items = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
        
        var tasksResponse = _mapper.Map<List<TaskResponse>>(items);
        
        return new PaginationResponse<TaskResponse>
        {
            Data = tasksResponse,
            Pagination = new PaginationMetadata
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                TotalRecords = totalRecords
            }
        };
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id, Guid userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task not found");
        if (!isAdmin && task.UserId != userId)
            throw new Exception("Access denied");
        
        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> CreateAsync(TaskRequest request, Guid userId)
    {
        if (request.DueDate < DateTime.UtcNow.Date)
            throw new Exception("Cannot create task with past due date");
        
        var task = _mapper.Map<TaskItem>(request);
        task.UserId = userId;
        task.CreatedAt = DateTime.UtcNow;
        task.Status = ETaskStatus.Pending;
        
        await _taskRepository.AddAsync(task);
        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, TaskRequest request, Guid userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task not found");
        if (!isAdmin && task.UserId != userId)
            throw new Exception("Access denied");
        
        _mapper.Map(request, task);
        task.UpdatedAt = DateTime.UtcNow;
        
        await _taskRepository.UpdateAsync(task);
        return _mapper.Map<TaskResponse>(task);
    }

    public async Task DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task not found");
        if (!isAdmin && task.UserId != userId)
            throw new Exception("Access denied");
        
        task.IsDeleted = true;
        await _taskRepository.UpdateAsync(task);
    }

    public async Task CompleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new Exception("Task not found");
        if (!isAdmin && task.UserId != userId)
            throw new Exception("Access denied");
        
        task.Status = ETaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);
    }

    public async Task RestoreAsync(Guid id, Guid userId, bool isAdmin)
    {
        var allTasks = await _taskRepository.FindAsync(t => t.Id == id);
        var task = allTasks.FirstOrDefault();
        
        if (task == null)
            throw new Exception("Task not found");
        if (!isAdmin && task.UserId != userId)
            throw new Exception("Access denied");
        
        task.IsDeleted = false;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);
    }
}
