using CadastroProdutos.Data;
using CadastroProdutos.DTO;
using CadastroProdutos.Services;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CadastroProdutos.Models;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;


namespace CadastroProdutos.Controllers
{
    [Authorize]
    public class EstoqueController : Controller

    {



        private readonly IEstoqueService _estoqueService;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public EstoqueController(IEstoqueService estoqueService, IConfiguration config, ApplicationDbContext context)
        {
            _estoqueService = estoqueService;
            _config = config;
            _context = context;
        }

        public IActionResult Index(string? busca) => View(_estoqueService.ObterProdutos(busca));

        public IActionResult ExportarExcel()
        {
            var bytes = _estoqueService.GerarPlanilhaExcel();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario.xlsx");
        }



        [HttpPost]
        public IActionResult EnviarPdfEmail(string? categoriaSelecionada)
        {
            var emailRemetente = _config["EmailSettings:Email"]!;
            var senhaApp = _config["EmailSettings:Senha"]!;

            _estoqueService.EnviarPdfPorEmail(categoriaSelecionada, emailRemetente, senhaApp);
            return RedirectToAction("Index");
        }


        public IActionResult SalvarProduto()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SalvarProduto(ProdutoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto); // Se os dados não forem válidos, volta para o form com os erros

            try
            {
                await _estoqueService.AdicionarProdutoAsync(dto);
                TempData["Mensagem"] = "Produto adicionado com sucesso!";

                // Só redireciona se deu tudo certo
                return RedirectToAction("Index", "estoque");
            }
            catch (Exception ex)
            {
                // Se deu erro no banco ou no serviço, exibe o erro na própria tela
                ModelState.AddModelError("", "Erro ao salvar produto: " + ex.Message);
                return View(dto); // Retorna a view mantendo os dados que o usuário digitou
            }

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            // Usamos o serviço para obter os dados, não o _context direto
            var produtoDto = await _estoqueService.ObterProdutoPorIdAsync(id);

            if (produtoDto == null)
            {
                return NotFound();
            }

            return View(produtoDto);
        }


        //AtualizarProdutoAsync
        [HttpPost]
        [ValidateAntiForgeryToken] // Garante a segurança contra ataques CSRF
        public async Task<IActionResult> Editar(ProdutoDto dto)
        {
            try
            {
                // Chama o método do serviço que já está corrigido para usar a Entidade real
                await _estoqueService.AtualizarProdutoAsync(dto);

                // Você pode adicionar uma mensagem de sucesso temporária usando TempData se quiser
                TempData["MensagemSucesso"] = "Produto excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao tentar excluir o produto: " + ex.Message;
            }

            // Redireciona de volta para a listagem atualizada
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Garante a segurança contra ataques CSRF
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                // Chama o método do serviço que já está corrigido para usar a Entidade real
                await _estoqueService.RemoverProdutoAsync(id);


                // Você pode adicionar uma mensagem de sucesso temporária usando TempData se quiser
                TempData["MensagemSucesso"] = "Produto excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao tentar excluir o produto: " + ex.Message;
            }

            // Redireciona de volta para a listagem atualizada
            return RedirectToAction(nameof(Index));
        }


    }
}
        


