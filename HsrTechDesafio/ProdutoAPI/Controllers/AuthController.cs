using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProdutoAPI.DTOS;
using ProdutoAPI.Services.Interfaces;

namespace ProdutoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                await _authService.RegisterAsync(request.Username, request.Password, request.Role);
                return Ok(new { message = "Usuário registrado com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
