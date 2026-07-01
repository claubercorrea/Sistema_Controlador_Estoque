using CadastroProdutos.DTO;
using CadastroProdutos.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CadastroProdutos.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ProdutoDto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fluent API: Configurações explícitas do EF Core para o Banco de Dados
            builder.Entity<ProdutoDto>(entity =>
            {
                entity.ToTable("Produtos");
                entity.HasKey(p => p.Id);   
                entity.Property(p => p.Preco).HasColumnType("decimal(18,2)");
            });
        }
    }
}