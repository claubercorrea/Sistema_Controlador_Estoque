//using CadastroProdutos.Models;
//using System.ComponentModel.DataAnnotations;
//using Microsoft.AspNetCore.Identity;


//namespace CadastroProdutos.DTO
//{
//    public class LoginDto
//    {
//        [Required(ErrorMessage = "O Email é obrigatório."), EmailAddress]
//        public string Email {  get; set; } = string.Empty;

//        [Required(ErrorMessage = " A senha é obrigatório")]
//        public string Senha { get; set;} = string.Empty;

//    }
//    public class RedefinirSenhaDto
//    {
//        [Required, EmailAddress]
//        public string Email { get; set; } = string.Empty;

//        [Required(ErrorMessage = " A nova senha é obrigatório "), MinLength(6)]
//        public string NovaSenha { get; set; } = string.Empty;
//        [Required, Compare("NovaSenha", ErrorMessage = " As senhas não conferem")]
//        public string ConfirmarSenha { get; set; } = string.Empty;
//    }

//    //public class CriarUsuarioDto
//    //{
//    //    [Required, EmailAddress] public string Email { get; set; } = string.Empty;

//    //    [Required, MinLength(6)] public string Senha { get; set; } = string.Empty;

//    //    public string? FotoUrl { get; set; }
//    //    [Required]
//    //    public string Role { get; set; } = string.Empty;
//    //}
//    public class CriarUsuarioDto
//    {
//        [Required(ErrorMessage = "O e-mail é obrigatório.")]
//        [EmailAddress(ErrorMessage = "E-mail inválido.")]
//        public required string Email { get; set; }

//        [Required(ErrorMessage = "A senha é obrigatória.")]
//        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
//        public required string Senha { get; set; }

//        public string Role { get; set; } = "Funcionario";

//        public string? FotoUrl { get; set; }
//    }
//    public class ExibirUsuarioDto
//    {
//        public string Id { get; set; } = string.Empty;
//        public string Email { get; set; } = string.Empty;
//        public string? FotoUrl { get; set; }
//        public string Role { get; set; } = string.Empty;
//        public bool PrimeiroAcesso { get; set; } 
//    }
//    public class ExibirProdutoDto
//    {
//        public int Id { get; set; }
//        public string Nome { get; set; } = string.Empty;
//        public int Quantidade { get; set; }
//        public decimal Preco { get; set; }
//        public string Codigo_Barra { get; set; } = string.Empty;
//        public string Categoria { get; set; } = string.Empty;


//    }
//    public class LoginDTO
//    {
//        public string Email { get; set; }
//        public string Senha { get; set; }
//    }

//    public class ProdutoDto
//    {
//        public int Id { get; set; }
//        public string Nome { get; set; } = string.Empty;
//        public decimal Preco { get; set; }
//        public int Quantidade { get; set; }
//        public string? Categoria { get; set; }
//        public string? Codigo_Barra { get; set; }
//    }
//}

using CadastroProdutos.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace CadastroProdutos.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class RedefinirSenhaDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória."), MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação é obrigatória.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
        public string ?Token { get; set; }
    }

    public class CriarUsuarioDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string Senha { get; set; } = string.Empty;

        public string Role { get; set; } = "Funcionario";

        public string? FotoUrl { get; set; }
    }

    public class ExibirUsuarioDto
    {
        public string Id { get; set; } = string.Empty; // <-- Corrigido para public
        public string Email { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
    }

    public class ExibirProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public string Codigo_Barra { get; set; } = string.Empty;
        public string ?Categoria { get; set; } = string.Empty;
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string ?Categoria { get; set; }
        public string?Codigo_Barra { get; set; }
    }
}