//using CadastroProdutos.DTO;
//using CadastroProdutos.Models;
//using Microsoft.AspNetCore.Identity;
//using System.Net;
//using System.Net.Mail;


//namespace CadastroProdutos.Services
//{
//    public interface IAdminService
//    {
//        Task<List<ExibirUsuarioDto>> ListarTodosUsuariosAsync();
//        Task<IdentityResult> CadastrarNovoUsuarioAsync(CriarUsuarioDto dto, string urlBaseSite);
//        Task AlterarNivelAcessoAsync(string userId, string novaRole);
//    }

//    public class AdminService : IAdminService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IConfiguration _config;

//        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _config = config;
//        }

//        public async Task<List<ExibirUsuarioDto>> ListarTodosUsuariosAsync()
//        {
//            var usuarios = _userManager.Users.ToList(); // EF Core mapeia tabela de usuários
//            var listaDto = new List<ExibirUsuarioDto>();

//            foreach (var user in usuarios)
//            {
//                var roles = await _userManager.GetRolesAsync(user);
//                listaDto.Add(new ExibirUsuarioDto
//                {
//                    Id = user.Id,
//                    Email = user.Email!,
//                    FotoUrl = user.FotoUrl,
//                    Role = roles.FirstOrDefault() ?? "Funcionario",
//                    PrimeiroAcesso = user.PrimeiroAcesso
//                });
//            }
//            return listaDto;
//        }

//        public async Task<IdentityResult> CadastrarNovoUsuarioAsync(CriarUsuarioDto dto, string urlBaseSite)
//        {
//            var novoUsuario = new ApplicationUser
//            {
//                UserName = dto.Email,
//                Email = dto.Email,
//                FotoUrl = dto.FotoUrl,
//                PrimeiroAcesso = true
//            };

//            var resultado = await _userManager.CreateAsync(novoUsuario, dto.Senha);

//            if (resultado.Succeeded)
//            {
//                if (!await _roleManager.RoleExistsAsync(dto.Role))
//                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));

//                await _userManager.AddToRoleAsync(novoUsuario, dto.Role);
//                EnviarEmailPrimeiroAcesso(dto.Email, dto.Senha, urlBaseSite);
//            }
//            return resultado;
//        }

//        public async Task AlterarNivelAcessoAsync(string userId, string novaRole)
//        {
//            var usuario = await _userManager.FindByIdAsync(userId);
//            if (usuario == null) return;

//            var rolesAtuais = await _userManager.GetRolesAsync(usuario);
//            await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
//            await _userManager.AddToRoleAsync(usuario, novaRole);
//        }

//        private void EnviarEmailPrimeiroAcesso(string emailDestinatario, string senhaProvisoria, string urlBaseSite)
//        {
//            var emailRemetente = _config["EmailSettings:Email"]!;
//            var senhaApp = _config["EmailSettings:Senha"]!;
//            string linkRedefinicao = $"{urlBaseSite}/Auth/RedefinirSenha?email={emailDestinatario}";

//            var mensagem = new MailMessage(emailRemetente, emailDestinatario)
//            {
//                Subject = "Acesso Criado - Altere sua Senha",
//                IsBodyHtml = true,
//                Body = $@"
//                    <h2>Bem-vindo ao Sistema!</h2>
//                    <p>Sua conta corporativa foi configurada.</p>
//                    <p><strong>Usuário:</strong> {emailDestinatario}</p>
//                    <p><strong>Senha Provisória:</strong> {senhaProvisoria}</p>
//                    <br/>
//                    <a href='{linkRedefinicao}' style='background-color:#0d6efd;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;display:inline-block;'>Alterar Minha Senha</a>"
//            };

//            using var smtp = new SmtpClient("smtp.gmail.com", 587);
//            smtp.Credentials = new NetworkCredential(emailRemetente, senhaApp);
//            smtp.EnableSsl = true;
//            smtp.Send(mensagem);
//        }
//    }
//}

using CadastroProdutos.DTO;
using CadastroProdutos.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;

namespace CadastroProdutos.Services
{
    public interface IAdminService
    {
        Task<List<ExibirUsuarioDto>> ListarTodosUsuariosAsync();
        Task<IdentityResult> CadastrarNovoUsuarioAsync(CriarUsuarioDto dto, string urlBaseSite);
        Task AlterarNivelAcessoAsync(string userId, string novaRole);
        Task AtualizarFotoUrlAsync(string userId, string novaUrlFoto);
        Task<IdentityResult> DeletarUsuarioAsync(string userId);

    }

    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public AdminService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _environment = environment;
        }

        public async Task<List<ExibirUsuarioDto>> ListarTodosUsuariosAsync()
        {
            var usuarios = _userManager.Users.ToList();
            var listaDto = new List<ExibirUsuarioDto>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);
                listaDto.Add(new ExibirUsuarioDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FotoUrl = user.FotoUrl,
                    Role = roles.FirstOrDefault() ?? "Funcionario",
                    PrimeiroAcesso = user.PrimeiroAcesso
                });
            }
            return listaDto;
        }

        public async Task<IdentityResult> CadastrarNovoUsuarioAsync(CriarUsuarioDto dto, string urlBaseSite)
        {
            var novoUsuario = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FotoUrl = dto.FotoUrl,
                PrimeiroAcesso = true
            };

            var resultado = await _userManager.CreateAsync(novoUsuario, dto.Senha);

            if (resultado.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(dto.Role))
                    await _roleManager.CreateAsync(new IdentityRole(dto.Role));

                await _userManager.AddToRoleAsync(novoUsuario, dto.Role);
                EnviarEmailPrimeiroAcesso(dto.Email, dto.Senha, urlBaseSite);
            }
            return resultado;
        }

        public async Task AlterarNivelAcessoAsync(string userId, string novaRole)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return;

            var rolesAtuais = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
            await _userManager.AddToRoleAsync(usuario, novaRole);
        }

        public async Task AtualizarFotoUrlAsync(string userId, string novaUrlFoto)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario != null)
            {
                usuario.FotoUrl = novaUrlFoto;
                await _userManager.UpdateAsync(usuario);
            }
        }

        private void EnviarEmailPrimeiroAcesso(string emailDestinatario, string senhaProvisoria, string urlBaseSite)
        {
            var emailRemetente = _config["EmailSettings:Email"]!;
            var senhaApp = _config["EmailSettings:Senha"]!;
            string linkRedefinicao = $"{urlBaseSite}/Auth/RedefinirSenha?email={emailDestinatario}";

            var mensagem = new MailMessage(emailRemetente, emailDestinatario)
            {
                Subject = "Acesso Criado - Altere sua Senha",
                IsBodyHtml = true,
                Body = $@"
                    <h2>Bem-vindo ao Sistema!</h2>
                    <p>Sua conta foi configurada com sucesso.</p>
                    <p><strong>Usuário:</strong> {emailDestinatario}</p>
                    <p><strong>Senha Provisória:</strong> {senhaProvisoria}</p>
                    <br/>
                    <a href='{linkRedefinicao}' style='background-color:#0d6efd;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>Alterar Senha</a>"
            };

            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(emailRemetente, senhaApp);
            smtp.EnableSsl = true;
            smtp.Send(mensagem);
        }

        public async Task<IdentityResult> DeletarUsuarioAsync(string userId)
        {
            // 1. Busca o usuário no banco de dados
            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            // 2. Remove o usuário do ASP.NET Identity (isso limpa as roles e logins associados a ele)
            var resultado = await _userManager.DeleteAsync(usuario);

            return resultado;
        }
    }
}