using System.ComponentModel.DataAnnotations;

namespace TodoList.Application.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}