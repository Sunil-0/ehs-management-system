using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/login -- the only endpoint that does NOT require a token
        // (everything else needs [Authorize], this is how you get one in the first place)
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result is null)
                return Unauthorized("Invalid email or password.");

            return Ok(result);
        }
    }
}