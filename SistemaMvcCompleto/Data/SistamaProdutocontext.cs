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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MeuProduto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");
        }
    }
}