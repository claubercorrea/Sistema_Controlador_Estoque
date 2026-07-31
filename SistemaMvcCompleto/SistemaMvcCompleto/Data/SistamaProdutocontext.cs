using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaCompleto.Models;

namespace SistemaCompleto.Data
{
    public class SistamaProdutocontext : IdentityDbContext<User>
    {
        public SistamaProdutocontext(DbContextOptions<SistamaProdutocontext> options) 
            : base(options)
        { }

        public DbSet<MeuProduto> MeusProdutos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ⚠️ OBRIGATÓRIO para garantir as tabelas e relacionamentos do ASP.NET Identity
            base.OnModelCreating(modelBuilder);

            // 💰 Configura a precisão do Preço (18 dígitos no total, 2 casas decimais)
            modelBuilder.Entity<MeuProduto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");
        }
    }
}