using Microsoft.EntityFrameworkCore;
using SistemaCompleto.Data;
using SistemaCompleto.Models;
using SistemaCompleto.TDO;

namespace SistemaCompleto.Services
{
    public interface IProdutoServicecs
    {
        Task<IEnumerable<ProdutoDto>> GetAllProdutosAsync(string? pesquisa, string? categoria);

        Task<ProdutoDto?> GetProdutoByIdAsync(int id);
        Task CreateProdutoAsync(ProdutoDto produtoDto);
        Task UpdateProdutoAsync(ProdutoDto produtoDto);
    
        Task<bool> DeleteProdutoAsync(int id);
        Task DeleteAllProdutosAsync();
    }

    public class ProdutoService : IProdutoServicecs
    {
        private readonly SistamaProdutocontext _context;

        public ProdutoService(SistamaProdutocontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProdutoDto>> GetAllProdutosAsync(string? pesquisa, string? categoria)
        {
          

            var query =  _context.MeusProdutos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim().ToLower();
                query = query.Where(p => p.Nome_Produto.ToLower().Contains(pesquisa)
                                      || p.Categoria.ToLower().Contains(pesquisa)
                                     || p.Validade.ToString().Contains(pesquisa));
            }

       
            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(p => p.Categoria == categoria);
            }

            var produtos = await query.Select(p => new ProdutoDto
            {
                Id = p.Id,
                Nome = p.Nome_Produto,
                Categoria = p.Categoria,
                Preco = p.Preco,
                QuantidadeEstoque = p.Quantidade,
                Validade = p.Validade,
                Codigo_barra = p.Codigo_barra
            }).ToListAsync();

            return produtos;
        }
        

        public async Task<ProdutoDto?> GetProdutoByIdAsync(int id)
        {
            var produto = await _context.MeusProdutos.FindAsync(id);
            if (produto == null)
            {
                return null;
            }
            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome_Produto,
                Categoria = produto.Categoria,
                Preco = produto.Preco,
                Validade = produto.Validade,
                QuantidadeEstoque = produto.Quantidade,
                Codigo_barra = produto.Codigo_barra
            };
        }

        public async Task CreateProdutoAsync(ProdutoDto produtoDto)
        {
            var produto = new MeuProduto
            {
                Nome_Produto = produtoDto.Nome,
                Categoria = produtoDto.Categoria,
                Preco = produtoDto.Preco,
                Quantidade = produtoDto.QuantidadeEstoque,
                Validade = produtoDto.Validade,
                Codigo_barra = produtoDto.Codigo_barra,
                DataCadastro = DateTime.Now
            };

            _context.MeusProdutos.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProdutoAsync(ProdutoDto produtoDto)
        {
            var produto = await _context.MeusProdutos.FindAsync(produtoDto.Id);
            if (produto != null)
            {
                produto.Nome_Produto = produtoDto.Nome;
                produto.Categoria = produtoDto.Categoria;
                produto.Preco = produtoDto.Preco;
                produto.Quantidade = produtoDto.QuantidadeEstoque;
                produto.Validade = produtoDto.Validade;
                produto.Codigo_barra = produtoDto.Codigo_barra;

                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _context.MeusProdutos.FindAsync(id);

            if (produto == null)
                return false;

            _context.MeusProdutos.Remove(produto);
            await _context.SaveChangesAsync();

            var tabelaVazia = !await _context.MeusProdutos.AnyAsync();
            if (tabelaVazia)
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MeusProdutos', RESEED, 0)");
            }

            return true;
        }


        public async Task DeleteAllProdutosAsync()
        {
            var produtos = await _context.MeusProdutos.ToListAsync();

            if (produtos.Any())
            {
                _context.MeusProdutos.RemoveRange(produtos);
                await _context.SaveChangesAsync();
            }

            var tabelaVazia = !await _context.MeusProdutos.AnyAsync();
            if (tabelaVazia)
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MeusProdutos', RESEED, 0)");
            }
        }
    }



}