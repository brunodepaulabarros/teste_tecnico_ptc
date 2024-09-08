using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.DTOs.UserDTO
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
