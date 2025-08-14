using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Data.Repositories
{
    public interface ILoginService
    {
        Task<User?> CreateUser(string name, string password);
        Task<User?> ValidateUser(string name, string password);
        string GenerateJWT(int id, string name, Roles role);
    }

    class LoginService : ILoginService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public LoginService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;

        }

        public async Task<User?> CreateUser(string name, string password)
        {
            //check if name is already in database

            bool userExist = await _dbContext.users.AnyAsync(u => u.Name == name);

            if (userExist)
            {
                return null;
            }

            string hashedPass = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Name = name,
                HashedPass = hashedPass,
                Role = Roles.User,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        async public Task<User?> ValidateUser(string name, string password)
        {
            User? user = await _dbContext.users.FirstOrDefaultAsync(u => u.Name == name);

            if (user == null)
            {
                return null;
            }

            bool isValidPass = BCrypt.Net.BCrypt.Verify(password, user.HashedPass);

            if (!isValidPass)
            {
                return null;
            }

            return user;
        }

        public string GenerateJWT(int id, string name, Roles role)
        {
            List<Claim> claims =
            [
                new (ClaimTypes.NameIdentifier, id.ToString()),
                new (ClaimTypes.Name, name),
                new (ClaimTypes.Role, role.ToString())
            ];


            var secret = _configuration["JwtSettings:Secret"] ?? "";

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

            return jwt;
        }
    }
}