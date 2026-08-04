using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaCompleto.Models;
using SistemaCompleto.TDO;

using SistemaMvcCompleto.TDO;
namespace SistemaMvcCompleto.Controllers
{
    [Authorize] 
    public class PerfilController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PerfilController(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound("Usuário não está logado ou não foi encontrado.");
            }

            var model = new PerfilDto
            {
                NomeCompleto = usuario.NomeCompleto ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Cargo = "Operador de Depósito",
                FotoUrlAtual = usuario.FotoUrl ?? string.Empty
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarPerfil(PerfilDto model, IFormFile? FotoPerfil)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (!ModelState.IsValid)
            {
                model.FotoUrlAtual = usuario.FotoUrl ?? string.Empty;
                return View("Perfil", model);
            }

          
            usuario.NomeCompleto = model.NomeCompleto;

      
            if (FotoPerfil != null && FotoPerfil.Length > 0)
            {
                string pastaPerfis = Path.Combine(_webHostEnvironment.WebRootPath, "img", "perfis");

                if (!Directory.Exists(pastaPerfis))
                {
                    Directory.CreateDirectory(pastaPerfis);
                }

                if (!string.IsNullOrEmpty(usuario.FotoUrl))
                {
                    string caminhoFotoAntiga = Path.Combine(_webHostEnvironment.WebRootPath, usuario.FotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoFotoAntiga))
                    {
                        System.IO.File.Delete(caminhoFotoAntiga);
                    }
                }

                string extensao = Path.GetExtension(FotoPerfil.FileName);
                string nomeUnicoArquivo = $"{Guid.NewGuid()}{extensao}";
                string caminhoCompletoArquivo = Path.Combine(pastaPerfis, nomeUnicoArquivo);

                using (var stream = new FileStream(caminhoCompletoArquivo, FileMode.Create))
                {
                    await FotoPerfil.CopyToAsync(stream);
                }

                usuario.FotoUrl = $"/img/perfis/{nomeUnicoArquivo}";
            }

            var resultadoUpdate = await _userManager.UpdateAsync(usuario);
            if (!resultadoUpdate.Succeeded)
            {
                AdicionarErrosIdentity(resultadoUpdate);
                model.FotoUrlAtual = usuario.FotoUrl ?? string.Empty;
                return View("Perfil", model);
            }

            if (!string.IsNullOrEmpty(model.SenhaAtual) && !string.IsNullOrEmpty(model.NovaSenha))
            {
                var resultadoSenha = await _userManager.ChangePasswordAsync(usuario, model.SenhaAtual, model.NovaSenha);

                if (!resultadoSenha.Succeeded)
                {
                    AdicionarErrosIdentity(resultadoSenha);
                    model.FotoUrlAtual = usuario.FotoUrl ?? string.Empty;
                    return View("Perfil", model);
                }
            }

            TempData["MensagemSucesso"] = "Perfil e credenciais atualizados com sucesso!";
            return RedirectToAction("Index", "Produto");
        }

        private void AdicionarErrosIdentity(IdentityResult result)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError(string.Empty, erro.Description);
            }
        }

    }


}


