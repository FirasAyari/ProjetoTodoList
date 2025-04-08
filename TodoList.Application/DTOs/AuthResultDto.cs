using System;
using System.Collections.Generic;
using System.Linq; 

namespace TodoList.Application.DTOs
{
    public class AuthResultDto 
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public UserDto? User { get; set; }
    }
}