using CadastroProdutos.Data;
using CadastroProdutos.DTO;
using CadastroProdutos.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Net;
using System.Net.Mail;

namespace CadastroProdutos.Services
{
    public interface IEstoqueService
    {
        List<ExibirProdutoDto> ObterProdutos(string? busca);
        byte[] GerarPlanilhaExcel();
        void EnviarPdfPorEmail(string? categoria, string emailRemetente, string senhaApp);

        // Métodos do CRUD usando ProdutoDto
        Task AdicionarProdutoAsync(ProdutoDto dto);
        Task AtualizarProdutoAsync(ProdutoDto dto);
        Task RemoverProdutoAsync(int id);

        // Remova o modificador 'async' da assinatura do método na interface
        public Task<ProdutoDto> ObterProdutoPorIdAsync(int id);
    }

    public class EstoqueService : IEstoqueService
    {
        private readonly ApplicationDbContext _context;

        public EstoqueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ExibirProdutoDto> ObterProdutos(string? busca)
        {
            var query = _context.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(busca))
            {
                query = query.Where(p =>
                    (p.Nome != null && p.Nome.Contains(busca)) ||
                    (p.Categoria != null && p.Categoria.Contains(busca))
                );
            }

            return query.Select(p => new ExibirProdutoDto
            {
                Id = p.Id,
                Nome = p.Nome ?? "",
                Quantidade = p.Quantidade,
                Preco = p.Preco,
                Categoria = p.Categoria ?? "",
                Codigo_Barra = p.Codigo_Barra ?? ""
            }).ToList();
        }

        public byte[] GerarPlanilhaExcel()
        {
            var dados = _context.Produtos.ToList();
            using var planilha = new XLWorkbook();
            var aba = planilha.Worksheets.Add("Estoque");

            aba.Cell(1, 1).Value = "Nome";
            aba.Cell(1, 2).Value = "Quantidade";
            aba.Cell(1, 3).Value = "Preço";
            aba.Cell(1, 4).Value = "Codigo_Barra";

            for (int i = 0; i < dados.Count; i++)
            {
                aba.Cell(i + 2, 1).Value = dados[i].Nome;
                aba.Cell(i + 2, 2).Value = dados[i].Quantidade;
                aba.Cell(i + 2, 3).Value = dados[i].Preco.ToString("F2");
                aba.Cell(i + 2, 4).Value = dados[i].Codigo_Barra;
            }

            using var stream = new MemoryStream();
            planilha.SaveAs(stream);
            return stream.ToArray();
        }

        public void EnviarPdfPorEmail(string? categoria, string emailRemetente, string senhaApp)
        {
            var query = _context.Produtos.AsQueryable();
            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => p.Categoria == categoria);
            }
            var produtos = query.ToList();

            var bytesPdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Header().Text("Relatório Comercial de Estoque").FontSize(16).Bold();
                    page.Content().Table(tabela =>
                    {
                        tabela.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
                        foreach (var p in produtos)
                        {
                            tabela.Cell().Text(p.Nome ?? "");
                            tabela.Cell().Text(p.Quantidade.ToString());
                            tabela.Cell().Text(p.Codigo_Barra ?? "");
                        }
                    });
                });
            }).GeneratePdf();

            var mensagem = new MailMessage(emailRemetente, emailRemetente)
            {
                Subject = "Relatório PDF em Anexo",
                Body = "Planilha de dados gerada automaticamente via EF Core."
            };

            mensagem.Attachments.Add(new Attachment(new MemoryStream(bytesPdf), "Estoque.pdf"));

            using var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(emailRemetente, senhaApp);
            smtp.EnableSsl = true;
            smtp.Send(mensagem);
        }

        public async Task AdicionarProdutoAsync(ProdutoDto dto)
        {
            // O operador ! avisa ao compilador que os campos obrigatórios validados não virão nulos
            var produto = new ProdutoDto
            {
                Nome = dto.Nome!,
                Quantidade = dto.Quantidade,
                Preco = dto.Preco,
                Categoria = dto.Categoria!,
                Codigo_Barra = dto.Codigo_Barra ?? ""
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarProdutoAsync(ProdutoDto dto)
        {
            var produto = await _context.Produtos.FindAsync(dto.Id);

            if (produto != null)
            {
                produto.Nome = dto.Nome!;
                produto.Quantidade = dto.Quantidade;
                produto.Preco = dto.Preco;
                produto.Categoria = dto.Categoria!;
                produto.Codigo_Barra = dto.Codigo_Barra ?? "";

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                // Verifica se a tabela ficou vazia após a exclusão
                var totalProdutos = await _context.Produtos.CountAsync();

                if (totalProdutos == 0)
                {
                    // Reseta o contador (IDENTITY) para 0, assim o próximo será 1
                    await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Produtos', RESEED, 0)");
                }
            }
        }

        public async Task<ProdutoDto> ObterProdutoPorIdAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return null!;

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Quantidade = produto.Quantidade,
                Preco = produto.Preco,
                Categoria = produto.Categoria,
                Codigo_Barra = produto.Codigo_Barra
            };
        }
    }
    }

    
