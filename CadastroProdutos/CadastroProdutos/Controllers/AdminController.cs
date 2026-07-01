using CadastroProdutos.DTO;
using CadastroProdutos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroProdutos.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IWebHostEnvironment _environment;

        // Construtor sem o atributo [Authorize]
        public AdminController(IAdminService adminService, IWebHostEnvironment environment)
        {
            _adminService = adminService;
            _environment = environment;
        }

        public async Task<IActionResult> Painel() => View(await _adminService.ListarTodosUsuariosAsync());

        [HttpGet] public IActionResult Cadastrar() => View();

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CriarUsuarioDto dto, IFormFile? fotoArquivo)
        {
            if (!ModelState.IsValid) return View(dto);

            if (fotoArquivo != null && fotoArquivo.Length > 0)
            {
                string pastaPerfis = Path.Combine(_environment.WebRootPath, "img", "perfis");
                if (!Directory.Exists(pastaPerfis)) Directory.CreateDirectory(pastaPerfis);

                string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoArquivo.FileName);
                string caminhoCompleto = Path.Combine(pastaPerfis, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await fotoArquivo.CopyToAsync(stream);
                }
                dto.FotoUrl = $"/img/perfis/{nomeArquivo}";
            }

            string urlBase = $"{Request.Scheme}://{Request.Host}";
            var resultado = await _adminService.CadastrarNovoUsuarioAsync(dto, urlBase);

            if (resultado.Succeeded) return RedirectToAction("Painel");

            foreach (var erro in resultado.Errors) ModelState.AddModelError("", erro.Description);
            return View(dto);
        }

       [HttpPost]
public async Task<IActionResult> AlterarPermissao(string userId, string novaRole) 
{
    // 1. TRAVA DE SEGURANÇA PRIMEIRO: Se 'novaRole' for vazia, para a execução aqui mesmo!
    if (string.IsNullOrWhiteSpace(novaRole))
    {
        TempData["MensagemErro"] = "O nível de acesso selecionado é inválido.";
        return RedirectToAction("Painel");
    }

    // 2. SÓ EXECUTA SE PASSAR NA VALIDAÇÃO: Agora é seguro chamar o serviço
    await _adminService.AlterarNivelAcessoAsync(userId, novaRole);

    TempData["MensagemSucesso"] = "Permissão alterada com sucesso!";
    return RedirectToAction("Painel");
}
        [HttpPost]
        public async Task<IActionResult> AlterarNivelAcesso(string userId, string novaRole)
        {
            await _adminService.AlterarNivelAcessoAsync(userId, novaRole);
            return RedirectToAction("Painel");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Boa prática de segurança para requisições POST
        public async Task<IActionResult> AlterarFotoPerfil(string userId, IFormFile fotoArquivo)
        {
            // Verifica se o arquivo foi enviado corretamente
            if (fotoArquivo == null || fotoArquivo.Length == 0)
            {
                TempData["MensagemErro"] = "Nenhum arquivo de imagem foi selecionado.";
                return RedirectToAction("Painel");
            }

            // OTIMIZAÇÃO: Busque apenas o usuário específico pelo ID em vez de listar todos do banco
            // Se o seu IAdminService tiver um método dedicado, use-o. Caso contrário, use a linha abaixo:
            var usuarios = await _adminService.ListarTodosUsuariosAsync();
            var usuarioAtual = usuarios.FirstOrDefault(u => u.Id == userId);

            if (usuarioAtual == null) return NotFound();

            try
            {
                // 1. Apagar foto antiga do servidor físico (se ela existir e for um caminho válido)
                if (!string.IsNullOrEmpty(usuarioAtual.FotoUrl) && !usuarioAtual.FotoUrl.Contains("cdn-icons-png"))
                {
                    string caminhoAntigo = Path.Combine(_environment.WebRootPath, usuarioAtual.FotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(caminhoAntigo))
                    {
                        System.IO.File.Delete(caminhoAntigo);
                    }
                }

                // 2. Criar a pasta wwwroot/img/perfis/ se ela não existir
                string pastaPerfis = Path.Combine(_environment.WebRootPath, "img", "perfis");
                if (!Directory.Exists(pastaPerfis)) Directory.CreateDirectory(pastaPerfis);

                // 3. Gerar nome único com GUID para evitar que imagens com o mesmo nome se sobrescrevam
                string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoArquivo.FileName);
                string caminhoCompleto = Path.Combine(pastaPerfis, nomeArquivo);

                // 4. Salvar o novo arquivo no disco
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await fotoArquivo.CopyToAsync(stream);
                }

                // 5. Atualizar no banco usando caminho relativo para o navegador renderizar
                       

                        // Por esta linha correta:
                        string novaUrlFoto = $"/wwwroot/i/perfis/{nomeArquivo}";
                await _adminService.AtualizarFotoUrlAsync(userId, novaUrlFoto);

                TempData["MensagemSucesso"] = "Foto de perfil atualizada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao processar a foto de perfil: " + ex.Message;
            }

            return RedirectToAction("Estoque","Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarUsuario(string userId)
        {
            // 1. Segurança máxima: Impede o Admin de deletar a si próprio
            var adminLogadoId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == adminLogadoId)
            {
                TempData["MensagemErro"] = "Você não pode excluir a sua própria conta de administrador!";
                return RedirectToAction("Painel");
            }

            // 2. Executa a exclusão através do serviço
            var resultado = await _adminService.DeletarUsuarioAsync(userId);

            if (resultado.Succeeded)
            {
                TempData["MensagemSucesso"] = "Conta de login excluída com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Erro ao tentar excluir a conta.";
            }

            return RedirectToAction("Painel");
        }
    }
}
