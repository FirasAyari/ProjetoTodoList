using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.Data;
using TodoList.Infrastructure.Services;
using Xunit;

namespace TodoList.Tests
{
    public class TaskServiceTests
    {
        private DbContextOptions<TodoListDbContext> GetInMemoryDbContextOptions(string dbName)
        {
            return new DbContextOptionsBuilder<TodoListDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public async Task AddTaskAsync_ShouldAddTaskForCorrectUser()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = GetInMemoryDbContextOptions(dbName);
            var userId = "test-user-1";
            var createTaskDto = new CreateTaskDto { Title = "Test Task", Description = "Test Desc" };

            using (var context = new TodoListDbContext(options))
            {
                var service = new TaskService(context);
                var resultDto = await service.AddTaskAsync(userId, createTaskDto);

                Assert.NotNull(resultDto);
                Assert.True(resultDto.Id > 0);
                Assert.Equal(createTaskDto.Title, resultDto.Title);
            }

            using (var context = new TodoListDbContext(options))
            {
                var savedTask = await context.TaskItems.FirstOrDefaultAsync(t => t.Title == "Test Task");
                Assert.NotNull(savedTask);
                Assert.Equal(userId, savedTask.UserId);
                Assert.False(savedTask.IsCompleted);
            }
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnOnlyUserTasks_AndPaginate()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = GetInMemoryDbContextOptions(dbName);
            var userId1 = "user-tasks-1";
            var userId2 = "user-tasks-2";

            using (var context = new TodoListDbContext(options))
            {
                context.TaskItems.AddRange(
                    new TaskItem { Title = "Task 1", UserId = userId1 },
                    new TaskItem { Title = "Task 2", UserId = userId1 },
                    new TaskItem { Title = "Task 3", UserId = userId1 },
                    new TaskItem { Title = "Other User Task", UserId = userId2 }
                );
                await context.SaveChangesAsync();
            }

            using (var context = new TodoListDbContext(options))
            {
                var service = new TaskService(context);

                var resultPage1 = await service.GetTasksAsync(userId1, 1, 2);

                Assert.NotNull(resultPage1);
                Assert.Equal(2, resultPage1.Items.Count);
                Assert.Equal(1, resultPage1.PageNumber);
                Assert.Equal(2, resultPage1.TotalPages);
                Assert.Equal(3, resultPage1.TotalCount);
                Assert.Equal("Task 1", resultPage1.Items[0].Title);
                Assert.Equal("Task 2", resultPage1.Items[1].Title);
                Assert.True(resultPage1.HasNextPage);
                Assert.False(resultPage1.HasPreviousPage);

                var resultPage2 = await service.GetTasksAsync(userId1, 2, 2);

                Assert.NotNull(resultPage2);
                Assert.Single(resultPage2.Items);
                Assert.Equal(2, resultPage2.PageNumber);
                Assert.Equal(2, resultPage2.TotalPages);
                Assert.Equal(3, resultPage2.TotalCount);
                Assert.Equal("Task 3", resultPage2.Items[0].Title);
                Assert.False(resultPage2.HasNextPage);
                Assert.True(resultPage2.HasPreviousPage);
            }
        }
    }
}