using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data.DTO.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public LoginController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<StandardResponse>> Login(CreateUserDto userDto)
        {
            User? user = await _dbContext.users.FirstOrDefaultAsync(u => u.Name.ToLower() == userDto.Name.ToLower());


            if (user == null)
            {
                return Unauthorized(new StandardResponse { Message = "Credenciales incorrectas" });
            }

            bool isValidPass = BCrypt.Net.BCrypt.Verify(userDto.Password, user.HashedPass);

            if (!isValidPass)
            {
                return Unauthorized(new StandardResponse { Message = "Credenciales incorrectas" });
            }

            //Generate jwt

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

            var secret = _configuration["JwtSettings:Secret"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { Token = jwt });
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
