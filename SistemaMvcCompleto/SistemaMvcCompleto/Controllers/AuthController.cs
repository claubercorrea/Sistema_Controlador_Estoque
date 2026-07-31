using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaCompleto.Models;
using SistemaCompleto.Services;

namespace SistemaMvcCompleto.Controllers
{

    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthApiController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Senha, false);
            if (!result.Succeeded)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            // 🎯 Gera o JWT
            var token = await _tokenService.GerarTokenAsync(user);

            return Ok(new
            {
                token = token,
                expiration = DateTime.UtcNow.AddHours(8),
                user = new { user.Email, user.NomeCompleto }
            });
        }
    }

    public record LoginApiRequest(string Email, string Senha);
}
