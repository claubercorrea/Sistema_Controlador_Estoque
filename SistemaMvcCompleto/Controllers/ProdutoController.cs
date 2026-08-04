using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCompleto.Data;
using SistemaCompleto.Models;
using SistemaCompleto.Services;
using SistemaCompleto.TDO;

namespace SistemaMvcCompleto.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        private readonly IProdutoServicecs _produtoService;
        private readonly IExcelService _excelService;
        private readonly SistamaProdutocontext _context;

        public ProdutoController(IProdutoServicecs produtoService, IExcelService excelService, SistamaProdutocontext context)
        {
            _produtoService = produtoService;
            _excelService = excelService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? Pesquisa, string? Categoria)
        {
            var produtos = await _produtoService.GetAllProdutosAsync(Pesquisa, Categoria);

            ViewData["CategoriaAtual"] = Categoria;
            ViewData["FiltroAtual"] = Pesquisa;

            bool temFiltroAtivo = !string.IsNullOrWhiteSpace(Pesquisa) || !string.IsNullOrWhiteSpace(Categoria);

            if (!produtos.Any() && temFiltroAtivo)
            {
                TempData["Aviso"] = "Nenhum produto foi encontrado com os filtros informados.";
            }

            return View(produtos);
        }


        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(ProdutoDto dto)
        {
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            try
            {
                await _produtoService.CreateProdutoAsync(dto);
                TempData["Mensagem"] = "Produto criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar o produto no banco de dados: " + ex.Message);
                return View("Create", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var produto = await _produtoService.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProdutoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _produtoService.UpdateProdutoAsync(dto);
            TempData["Mensagem"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

[Authorize(Roles = "Admin")]
[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var sucesso = await _produtoService.DeleteProdutoAsync(id);
                if (sucesso)
                {
                    TempData["MensagemSucesso"] = "Produto excluído com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Produto não encontrado para exclusão.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao excluir o produto: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTodos()
        {
            try
            {
                await _produtoService.DeleteAllProdutosAsync();
                TempData["MensagemSucesso"] = "Todos os produtos foram excluídos e o contador de ID foi resetado!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao excluir todos os produtos: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportarExcel(string? pesquisa, string? categoria)
        {
            var produtos = await _produtoService.GetAllProdutosAsync(pesquisa, categoria);

            if (produtos == null || !produtos.Any())
            {
                TempData["Aviso"] = "Sua base de produtos está vazia no momento.";
                return RedirectToAction(nameof(Index));
            }

            var excelData = await _excelService.ExportToExcelAsync(produtos);

            return File(
                excelData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Produtos.xlsx"
            );
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Vencimentos(string pesquisa, string categoria, string statusVencimento)
        {
            var produtos = await _produtoService.GetAllProdutosAsync(pesquisa, categoria);
            var query = produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(p => p.Nome.Contains(pesquisa, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(p => p.Categoria == categoria);
            }

            DateTime hoje = DateTime.Now.Date;

            switch (statusVencimento)
            {
                case "vencidos":
                    query = query.Where(p => p.Validade.Date < hoje);
                    break;
                case "aVencer":
                    var limiteFuturo = hoje.AddDays(30);
                    query = query.Where(p => p.Validade.Date > hoje && p.Validade.Date <= limiteFuturo);
                    break;
                case "hoje":
                    query = query.Where(p => p.Validade.Date == hoje);
                    break;
                default:
                    var dataLimite = hoje.AddDays(30);
                    query = query.Where(p => p.Validade.Date <= dataLimite);
                    break;
            }

            ViewData["FiltroAtual"] = pesquisa;
            ViewData["CategoriaAtual"] = categoria;
            ViewData["StatusAtual"] = string.IsNullOrEmpty(statusVencimento) ? "todos" : statusVencimento;

            var listaFiltrada = query.OrderBy(p => p.Validade).ToList();

            return View("Vencimentos", listaFiltrada);
        }

       
        [Authorize]
        public async Task<IActionResult> EstoqueBaixo(string? Pesquisa, string? Categoria)
        {
            var produtos = await _produtoService.GetAllProdutosAsync(Pesquisa, Categoria);

            var produtosBaixos = produtos
                .Where(p => p.QuantidadeEstoque <= 10)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToList();

            ViewData["CategoriaAtual"] = Categoria;
            ViewData["FiltroAtual"] = Pesquisa;

            bool temFiltroAtivo = !string.IsNullOrWhiteSpace(Pesquisa) || !string.IsNullOrWhiteSpace(Categoria);

            if (!produtosBaixos.Any() && temFiltroAtivo)
            {
                TempData["Aviso"] = "Nenhum produto em estoque baixo foi encontrado com os filtros informados.";
            }

            return View(produtosBaixos);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportarEstoqueBaixo()
        {
            var todosProdutos = await _produtoService.GetAllProdutosAsync(null, null);

            var estoqueBaixo = todosProdutos
                .Where(p => p.QuantidadeEstoque <= 10)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Estoque Baixo");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Nome do Produto";
            worksheet.Cell(1, 3).Value = "Categoria";
            worksheet.Cell(1, 4).Value = "Quantidade";
            worksheet.Cell(1, 5).Value = "Validade";

            var header = worksheet.Range("A1:E1");
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#dc3545"); // Vermelho Alerta
            header.Style.Font.FontColor = XLColor.White;

            int linha = 2;
            foreach (var p in estoqueBaixo)
            {
                worksheet.Cell(linha, 1).Value = p.Id;
                worksheet.Cell(linha, 2).Value = p.Nome;
                worksheet.Cell(linha, 3).Value = p.Categoria;
                worksheet.Cell(linha, 4).Value = p.QuantidadeEstoque;

                if (p.Validade != DateTime.MinValue && p.Validade.Year >= 1900)
                {
                    worksheet.Cell(linha, 5).Value = p.Validade;
                    worksheet.Cell(linha, 5).Style.DateFormat.Format = "dd/MM/yyyy";
                }
                else
                {
                    worksheet.Cell(linha, 5).Value = "Data Inválida";
                }

                linha++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Estoque_Critico_{DateTime.Now:yyyyMMdd}.xlsx"
            );
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportarVencimentos(string pesquisa, string categoria, string statusVencimento)
        {
            var produtos = await _produtoService.GetAllProdutosAsync(pesquisa, categoria);
            var query = produtos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                query = query.Where(p => p.Nome.Contains(pesquisa, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(p => p.Categoria == categoria);
            }

            DateTime hoje = DateTime.Now.Date;

            switch (statusVencimento)
            {
                case "vencidos":
                    query = query.Where(p => p.Validade.Date < hoje);
                    break;
                case "aVencer":
                    var limiteFuturo = hoje.AddDays(30);
                    query = query.Where(p => p.Validade.Date > hoje && p.Validade.Date <= limiteFuturo);
                    break;
                case "hoje":
                    query = query.Where(p => p.Validade.Date == hoje);
                    break;
                default:
                    var dataLimite = hoje.AddDays(30);
                    query = query.Where(p => p.Validade.Date <= dataLimite);
                    break;
            }

            var listaVencimentos = query.OrderBy(p => p.Validade).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatorio Vencimentos");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Nome do Produto";
            worksheet.Cell(1, 3).Value = "Categoria";
            worksheet.Cell(1, 4).Value = "Quantidade";
            worksheet.Cell(1, 5).Value = "Validade";

            var header = worksheet.Range("A1:E1");
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffc107");
            header.Style.Font.FontColor = XLColor.Black;

            int linha = 2;
            foreach (var p in listaVencimentos)
            {
                worksheet.Cell(linha, 1).Value = p.Id;
                worksheet.Cell(linha, 2).Value = p.Nome;
                worksheet.Cell(linha, 3).Value = p.Categoria;
                worksheet.Cell(linha, 4).Value = p.QuantidadeEstoque;

                if (p.Validade != DateTime.MinValue && p.Validade.Year >= 1900)
                {
                    worksheet.Cell(linha, 5).Value = p.Validade;
                    worksheet.Cell(linha, 5).Style.DateFormat.Format = "dd/MM/yyyy";
                }
                else
                {
                    worksheet.Cell(linha, 5).Value = "Data Inválida";
                }

                linha++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            string nomeArquivo = $"Relatorio_Vencimentos_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
        }

    }
}
