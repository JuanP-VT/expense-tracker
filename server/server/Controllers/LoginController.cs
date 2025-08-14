using Microsoft.AspNetCore.Mvc;
using server.Data.DTO.Responses;
using server.Data.Repositories;

namespace server.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(IConfiguration configuration, ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(CreateUserDto userDto)
        {
            User? user = await _loginService.ValidateUser(userDto.Name, userDto.Password);


            if (user == null)
            {
                return Unauthorized(new StandardResponse { Message = "Credenciales incorrectas" });
            }

            string jwt = _loginService.GenerateJWT(user.Id, user.Name, user.Role);

            return Ok(new { Token = jwt });
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(CreateUserDto userDto)
        {
            User? user = await _loginService.CreateUser(userDto.Name, userDto.Password);

            if (user == null)
            {
                return Conflict("El usuario ya existe");
            }

            return Ok(new { Message = "¡Usuario creado!" });
        }
    }
}
