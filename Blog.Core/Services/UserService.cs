using Blog.Core.Interfaces;
using Blog.Domain.DTOs.UserDTO;
using Blog.Domain.Entities;
using Blog.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.Password == loginRequest.Password);

            if (user == null)
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Username
            };
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var user = new User
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                Username = registerRequest.UserName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName= user.Username
            };
        }
    }

}
