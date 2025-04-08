using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Application.Common; 
using TodoList.Application.DTOs;

namespace TodoList.Application.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedList<TaskDto>> GetTasksAsync(string userId, int pageNumber, int pageSize);
        Task<TaskDto?> GetTaskByIdAsync(string userId, long id);
        Task<TaskDto> AddTaskAsync(string userId, CreateTaskDto createTaskDto);
        Task<bool> UpdateTaskAsync(string userId, long id, UpdateTaskDto updateTaskDto);
        Task<bool> SetTaskCompletionAsync(string userId, long id, bool isCompleted); 
        Task<bool> DeleteTaskAsync(string userId, long id);
    }
}