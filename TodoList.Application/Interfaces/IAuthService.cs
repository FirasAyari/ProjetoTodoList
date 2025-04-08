using System.Threading.Tasks;
using TodoList.Application.DTOs;

namespace TodoList.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(UserRegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(UserLoginDto loginDto);
    }
}