using SistemaCompleto.Models;
using SistemaCompleto.TDO;
using System.ComponentModel.DataAnnotations;
namespace SistemaMvcCompleto.TDO
{
    public class PerfilDto
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Display(Name = "E-mail de Acesso")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Cargo / Função")]
        public string Cargo { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string? SenhaAtual { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter no mínimo {2} caracteres.")]
        public string? NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
        public string? ConfirmarNovaSenha { get; set; }

        public string? FotoUrlAtual { get; set; }
    }
}
