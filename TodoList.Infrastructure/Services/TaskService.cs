using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;

namespace TodoList.Infrastructure.Services
{
    public class TaskService : ITaskService
    {
        private readonly TodoListDbContext _context;

        public TaskService(TodoListDbContext context)
        {
            _context = context;
        }

        public async Task<TaskDto> AddTaskAsync(string userId, CreateTaskDto createTaskDto)
        {
            var taskItem = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                IsCompleted = false,
                UserId = userId
            };

                _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                IsCompleted = taskItem.IsCompleted
            };
        }

        public async Task<bool> DeleteTaskAsync(string userId, long id)
        {
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
            {
                return false;
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(string userId, long id)
        {
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
            {
                return null;
            }

            return new TaskDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                IsCompleted = taskItem.IsCompleted
            };
        }

        public async Task<PaginatedList<TaskDto>> GetTasksAsync(string userId, int pageNumber, int pageSize)
        {
            var query = _context.TaskItems
                                    .Where(t => t.UserId == userId)
                                    .OrderBy(t => t.Id)
                                    .AsNoTracking();

            var paginatedTaskItems = await PaginatedList<TaskItem>.CreateAsync(query, pageNumber, pageSize);

            var taskDtos = paginatedTaskItems.Items.Select(item => new TaskDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            }).ToList();

            return new PaginatedList<TaskDto>(
                taskDtos,
                paginatedTaskItems.TotalCount,
                paginatedTaskItems.PageNumber,
                pageSize);
        }

        public async Task<bool> SetTaskCompletionAsync(string userId, long id, bool isCompleted)
        {
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
            {
                return false;
            }

            taskItem.IsCompleted = isCompleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskAsync(string userId, long id, UpdateTaskDto updateTaskDto)
        {
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
            {
                return false;
            }

            taskItem.Title = updateTaskDto.Title;
            taskItem.Description = updateTaskDto.Description;
            taskItem.IsCompleted = updateTaskDto.IsCompleted;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}