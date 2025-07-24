using BurnoutTracker.Dtos;
using BurnoutTracker.Services;
using BurnoutTracker.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BurnoutTracker.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwt;
        private readonly IUserService _userService;

        public AuthController(JwtHelper jwt, IUserService userService)
        {
            _jwt = jwt;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var isValid = await _userService.ValidateCredentialsAsync(dto.Username, dto.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            var user = await _userService.GetUserByUsernameAsync(dto.Username);
            var token = _jwt.GenerateToken(user!);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDto dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto.Username, dto.Password);
                return Ok(new { message = "User registered successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
