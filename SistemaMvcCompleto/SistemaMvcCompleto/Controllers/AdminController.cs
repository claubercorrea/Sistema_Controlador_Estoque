using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCompleto.TDO; // 👈 Ajustado se mudou para DTOs
using SistemaCompleto.Models;

namespace SistemaMvcCompleto.Controllers
{
    // 🛡️ Proteção restrita para perfil Admin
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var model = new List<UsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                model.Add(new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email ?? string.Empty,
                    NomeCompleto = usuario.NomeCompleto,
                    Roles = roles.ToList(),
                    PrimeiroAcesso = usuario.PrimeiroAcesso
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarUsuario(string email, string nomeCompleto, string senha, string? role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nomeCompleto) || string.IsNullOrEmpty(senha))
            {
                TempData["MensagemErro"] = "Todos os campos obrigatórios devem ser preenchidos.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = new User
            {
                UserName = email,
                Email = email,
                NomeCompleto = nomeCompleto,
                EmailConfirmed = true, // 👈 Importante para liberar o login sem confirmação de e-mail
                PrimeiroAcesso = true  // 🔒 Exige alteração de senha no primeiro login
            };

            var resultado = await _userManager.CreateAsync(usuario, senha);
            if (resultado.Succeeded)
            {
                string perfilAtribuido = string.IsNullOrEmpty(role) ? "Usuario" : role;
                await _userManager.AddToRoleAsync(usuario, perfilAtribuido);

                TempData["Mensagem"] = $"Usuário criado com sucesso e associado ao perfil '{perfilAtribuido}'!";
            }
            else
            {
                TempData["MensagemErro"] = string.Join(", ", resultado.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirUsuario(string id)
        {
            var usuarioLogado = await _userManager.GetUserAsync(User);

            // 🛑 Impede que o Admin exclua a própria conta em uso
            if (usuarioLogado != null && usuarioLogado.Id == id)
            {
                TempData["MensagemErro"] = "Você não pode excluir seu próprio usuário logado.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var resultado = await _userManager.DeleteAsync(usuario);
            if (resultado.Succeeded)
            {
                TempData["Mensagem"] = "Usuário excluído com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = string.Join(", ", resultado.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetarSenha(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 💡 Usando uma senha temporária simplificada para os testes
            string senhaTemporaria = "123456";
            var resultadoReset = await _userManager.ResetPasswordAsync(user, token, senhaTemporaria);

            if (resultadoReset.Succeeded)
            {
                user.PrimeiroAcesso = true; // 🔒 Obriga troca após reset
                await _userManager.UpdateAsync(user);
                TempData["Mensagem"] = $"Senha do usuário {user.Email} foi resetada para '{senhaTemporaria}'.";
            }
            else
            {
                TempData["MensagemErro"] = string.Join(", ", resultadoReset.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }

    }
}