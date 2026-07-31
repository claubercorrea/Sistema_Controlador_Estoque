using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SistemaCompleto.Models;
using SistemaCompleto.TDO;

namespace SistemaCompleto.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Produto");
            }

            return View();
        }
        [HttpPost]
        [EnableRateLimiting("LoginLimit")] // 👈 Protege contra tentativas repetidas de senha

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado com este e-mail.");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, dto.Senha, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Se for primeiro acesso, redireciona para alterar a senha
                if (user.PrimeiroAcesso)
                {
                    return RedirectToAction(nameof(AlterarSenhaPrimeiroAcesso));
                }

                // Se houver ReturnUrl local válido, redireciona para ele
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Produto");
            }

            // Tratamento de erros para feedback em tela
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Conta bloqueada por muitas tentativas inválidas.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Usuário não tem permissão para logar (e-mail não confirmado?).");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Senha incorreta.");
            }

            return View(dto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AlterarSenhaPrimeiroAcesso()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var dto = new RedefinirSenhaDto
            {
                Email = user.Email ?? string.Empty
            };

            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenhaPrimeiroAcesso(RedefinirSenhaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // 1. Tenta alterar a senha no Identity
            var result = await _userManager.ChangePasswordAsync(user, dto.SenhaProvisoria, dto.NovaSenha);

            if (result.Succeeded)
            {
                // 2. Desmarca o primeiro acesso
                user.PrimeiroAcesso = false;
                await _userManager.UpdateAsync(user);

                // 3. Atualiza o cookie de sessão do usuário
                await _signInManager.RefreshSignInAsync(user);

                // 4. Redireciona para a página de Produtos
                return RedirectToAction("Index", "Produto");
            }

            // Se falhar, traduz o erro e exibe na View
            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordMismatch")
                {
                    ModelState.AddModelError(string.Empty, "A 'Senha Atual' digitada está incorreta.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    //await _signInManager.SignOutAsync();
        //    //return RedirectToAction(nameof(Login));
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Login", "Account");
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        //public async Task<IActionResult> Logout()
        //{
        //    // 🧼 Destrói a sessão e os cookies de autenticação
        //    await _signInManager.SignOutAsync();
        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
        //    {
        //        return Ok();
        //    }
        //    // 💬 Opcional: Mensagem exibida na tela de Login
        //    TempData["MensagemSucesso"] = "Sua sessão foi encerrada com sucesso.";

        //    return RedirectToAction("Login", "Account");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return Ok();
            }

            TempData["MensagemSucesso"] = "Sua sessão foi encerrada com sucesso.";
            return RedirectToAction("Login", "Account");
        }
    }
}