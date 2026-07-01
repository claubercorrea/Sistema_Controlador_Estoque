//using CadastroProdutos.DTO;
//using CadastroProdutos.Models;
//using CadastroProdutos.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace CadastroProdutos.Controllers
//{
//    // A injeção de dependência agora acontece diretamente na assinatura da classe
//    public class AuthController(
//        IAuthService authService,
//        SignInManager<ApplicationUser> signInManager) : Controller
//    {
//        [HttpGet]
//        public IActionResult Login() => View();

//        [HttpPost]
//        public async Task<IActionResult> Login(LoginDto loginDto)
//        {
//            if (!ModelState.IsValid) return View(loginDto);

//            // Verifica se é o primeiro acesso
//            if (await authService.VerificarPrimeiroAcessoAsync(loginDto.Email))
//            {
//                return RedirectToAction("RedefinirSenha", new { email = loginDto.Email });
//            }

//            // Autenticação oficial com Cookie
//            var resultado = await signInManager.PasswordSignInAsync(
//                loginDto.Email,
//                loginDto.Senha,
//                isPersistent: true,
//                lockoutOnFailure: false);

//            if (resultado.Succeeded)
//            {

//                return RedirectToAction("Index", "Estoque");
//            }

//            ModelState.AddModelError("", "Credenciais inválidas ou e-mail não confirmado.");
//            return View(loginDto);
//        }

//        [HttpGet]
//        public IActionResult RedefinirSenha(string email) => View(new RedefinirSenhaDto { Email = email });

//        [HttpPost]
//        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaDto dto)
//        {
//            if (!ModelState.IsValid) return View(dto);

//            var resultado = await authService.RedefinirSenhaPrimeiroAcessoAsync(dto);
//            if (resultado.Succeeded)
//            {
//                TempData["MensagemSucesso"] = "Senha alterada! Faça login com as novas credenciais.";
//                return RedirectToAction("Login");
//            }

//            foreach (var erro in resultado.Errors) ModelState.AddModelError("", erro.Description);
//            return View(dto);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Logout()
//        {
//            await signInManager.SignOutAsync();
//            return RedirectToAction("Login");
//        }
//    }
//}


using CadastroProdutos.DTO;
using CadastroProdutos.Models;
using CadastroProdutos.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace CadastroProdutos.Controllers
{
    // Construtor primário injetando as dependências corretamente
    public class AuthController(
        IAuthService authService,
        SignInManager<ApplicationUser> signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            // Se o usuário já estiver logado, manda direto para o estoque
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Estoque");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção de segurança essencial
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return View(loginDto);

            // 1. Intercepta se for o primeiro acesso para forçar a nova senha
            if (await authService.VerificarPrimeiroAcessoAsync(loginDto.Email))
            {
                return RedirectToAction("RedefinirSenha", new { email = loginDto.Email });
            }

            // 2. Autenticação oficial com Cookie do Identity
            var resultado = await signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Senha,
                isPersistent: true, // Mantém o usuário logado ao fechar o navegador
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Estoque");
            }

            ModelState.AddModelError("", "Credenciais inválidas ou conta bloqueada.");
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult RedefinirSenha(string email)
        {
            // Segurança: Se tentarem acessar a URL sem passar um e-mail, volta pro Login
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            return View(new RedefinirSenhaDto { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra falsificação de requisições
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            // Executa o serviço que remove a flag de primeiro acesso e atualiza a senha
            var resultado = await authService.RedefinirSenhaPrimeiroAcessoAsync(dto);

            if (resultado.Succeeded)
            {
                TempData["MensagemSucesso"] = "Senha alterada com sucesso! Faça login com suas novas credenciais.";
                return RedirectToAction("Login");
            }

            // Mapeia os erros de validação de senha do Identity (ex: falta de caractere especial) para a tela
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // Limpa o cookie de autenticação
            return RedirectToAction("Login");
        }
    }
}