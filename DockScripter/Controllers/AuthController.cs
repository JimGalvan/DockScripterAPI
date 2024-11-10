using Microsoft.AspNetCore.Mvc;
using ParkOnyx.Domain.Dtos.Requests;
using ParkOnyx.Services.Interfaces;

namespace ParkOnyx.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody]RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterUser(request, cancellationToken);
            if (!result)
                return BadRequest("Username already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login(LoginUserRequestDto request, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginUser(request, cancellationToken);
            if (token == null)
                return BadRequest("Invalid username or password.");

            return Ok(new { token });
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            _authService.LogoutUser(token);

            return Ok("User logged out successfully.");
        }
    }
}