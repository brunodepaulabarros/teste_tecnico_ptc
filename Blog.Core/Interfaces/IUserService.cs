using Blog.Domain.DTOs.UserDTO;

namespace Blog.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<UserResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    }
}
