using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.DTOs
{
    public class SetCompletionDto
    {
        [Required] 
        public bool IsCompleted { get; set; }
    }
}