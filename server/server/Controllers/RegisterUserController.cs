using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data.DTO.Responses;

namespace server.Controllers
{
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public RegisterUserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<StandardResponse>> RegisterUser(CreateUserDto userDto)
        {
            // Validate name
            string userDtoName = userDto.Name.Trim().ToLower();

            var userExists = await _dbContext.users
                .AnyAsync(u => u.Name.ToLower().Trim() == userDtoName);

            if (userExists)
            {
                return Conflict(new StandardResponse { Message = "User already exists" });
            }

            string hashedPass = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var newUser = new User
            {
                Name = userDto.Name,
                HashedPass = hashedPass,
                Role = (int)Roles.User,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return Ok(new StandardResponse { Message = "¡User Created!" });
        }
    }
}
