using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CadastroProdutos.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FotoUrl { get; set; } // Coluna extra na tabela de usuários

        // Flag que controla se o usuário precisa redefinir a senha gerada pelo administrador
        public bool PrimeiroAcesso { get; set; } = true;
    }

  
        public class RedefinirSenhaViewModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "A nova senha é obrigatória.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nova Senha")]
            public string NovaSenha { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Nova Senha")]
            [Compare("NovaSenha", ErrorMessage = "A senha e a confirmação não coincidem.")]
            public string ConfirmarSenha { get; set; } = string.Empty;
        }
 
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string? Categoria { get; set; }
        public string? Codigo_Barra { get; set; }
    }
}