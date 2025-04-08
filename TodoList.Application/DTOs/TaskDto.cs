namespace TodoList.Application.DTOs
{
    public class TaskDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}