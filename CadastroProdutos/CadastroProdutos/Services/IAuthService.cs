using CadastroProdutos.DTO;
using CadastroProdutos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CadastroProdutos.Services
{
    public interface IAuthService
    {
        Task<string?> AutenticarUsuarioAsync(LoginDto loginDto);
        Task<bool> VerificarPrimeiroAcessoAsync(string email);
        Task<IdentityResult> RedefinirSenhaPrimeiroAcessoAsync(RedefinirSenhaDto dto);
    }
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        public async Task<bool> VerificarPrimeiroAcessoAsync(string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            return usuario != null && usuario.PrimeiroAcesso;
        }
        public async Task<string?> AutenticarUsuarioAsync(LoginDto loginDto)
        {
            var usuario = await _userManager.FindByEmailAsync(loginDto.Email);
            if (usuario == null) return null;

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, loginDto.Senha, false);
            if (!resultado.Succeeded) return null;

            var roles = await _userManager.GetRolesAsync(usuario);
            var rolePrincipal = roles.FirstOrDefault() ?? "Funcionario";

            return GerarTokenJwt(usuario, rolePrincipal);
        }
        public async Task<IdentityResult> RedefinirSenhaPrimeiroAcessoAsync(RedefinirSenhaDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario invalido. " });
            }
            var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultado = await _userManager.ResetPasswordAsync(usuario, tokenReset, dto.NovaSenha);
            if (resultado.Succeeded)
            {
                usuario.PrimeiroAcesso = false;
                await _userManager.UpdateAsync(usuario); // EF Core persiste a alteração da flag
            }
            return resultado;
        }
        private string GerarTokenJwt(ApplicationUser usuario, string role)
        {
            var manipuladorToken = new JwtSecurityTokenHandler();
            var chave = Encoding.UTF8.GetBytes(_config["JwtSettings:ChaveSecreta"]!);

            var descritorToken = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, usuario.UserName!),
                    new Claim(ClaimTypes.Email, usuario.Email!),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("~/imagem/perfil", usuario.FotoUrl ?? "https://cdn-icons-png.flaticon.com/512/149/149071.png")
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenCriado = manipuladorToken.CreateToken(descritorToken);
            return manipuladorToken.WriteToken(tokenCriado);
        }


    }
}



