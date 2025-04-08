using System.ComponentModel.DataAnnotations;

namespace TodoList.Domain.Entities
{
    public class TaskItem
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        [Required]
        public string UserId { get; set; }

    }
}