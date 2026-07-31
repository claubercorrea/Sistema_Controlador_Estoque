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
        //Task DeleteProdutoAsync(int id);
        //Task DeleteAllProdutosAsync(MeuProduto produto);
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
            //return await _context.MeusProdutos
            //    .Select(p => new ProdutoDto
            //    {
            //        Id = p.Id,
            //        Nome = p.Nome_Produto,
            //        Categoria = p.Categoria,
            //        Preco = p.Preco,
            //        QuantidadeEstoque = p.Quantidade,
            //        Validade = p.Validade,
            //        Codigo_barra = p.Codigo_barra
            //    })
            //    .ToListAsync();

            var query =  _context.MeusProdutos.AsQueryable();

            // Filtro por Nome ou Código de Barras
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim().ToLower();
                query = query.Where(p => p.Nome_Produto.ToLower().Contains(pesquisa)
                                      || p.Categoria.ToLower().Contains(pesquisa)
                                     || p.Validade.ToString().Contains(pesquisa));
            }

            // Filtro por Categoria
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
        //public async Task DeleteAllProdutosAsync(MeuProduto produto)
        //{
        //    // Remove todos os registros da tabela de produtos de forma otimizada
        //    var produtos = await _context.MeusProdutos.ToListAsync();

        //    if (produtos.Any())
        //    {
        //        _context.MeusProdutos.RemoveRange(produtos);
        //        await _context.SaveChangesAsync();


        //        // Por apenas:
        //        await _context.SaveChangesAsync();

        //    }

        //    // Reseta o auto-incremento caso a tabela fique vazia
        //    var tabelaVazia = !await _context.MeusProdutos.AnyAsync();
        //    if (tabelaVazia)
        //    {
        //        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Produtos', RESEED, 0)");
        //    }
        //}

        //public async Task DeleteProdutoAsync(int id)
        //{
        //    var produto = await _context.MeusProdutos.FindAsync(id);
        //    if (produto != null)
        //    {
        //        _context.MeusProdutos.Remove(produto);
        //        await _context.SaveChangesAsync();
        //    }

        //}

        // 🗑️ Excluir um único produto por ID
        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _context.MeusProdutos.FindAsync(id);

            if (produto == null)
                return false;

            _context.MeusProdutos.Remove(produto);
            await _context.SaveChangesAsync();

            // Se a tabela ficou vazia após essa exclusão, reseta o ID para 0
            var tabelaVazia = !await _context.MeusProdutos.AnyAsync();
            if (tabelaVazia)
            {
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MeusProdutos', RESEED, 0)");
            }

            return true;
        }

        // ⚠️ Excluir TODOS os produtos do banco
        public async Task DeleteAllProdutosAsync()
        {
            var produtos = await _context.MeusProdutos.ToListAsync();

            if (produtos.Any())
            {
                _context.MeusProdutos.RemoveRange(produtos);
                await _context.SaveChangesAsync();
            }

            // Garantia extra: se a tabela estiver vazia, reseta a Sequence/Identity do SQL Server
            var tabelaVazia = !await _context.MeusProdutos.AnyAsync();
            if (tabelaVazia)
            {
                // Certifique-se de que 'MeusProdutos' seja o nome exato da TABELA no seu Banco SQL Server
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('MeusProdutos', RESEED, 0)");
            }
        }
    }



}