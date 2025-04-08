using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

    }
}