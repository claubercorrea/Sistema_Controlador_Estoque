using System.ComponentModel.DataAnnotations;

namespace SistemaCompleto.TDO
{
    public class RedefinirSenhaDto
    {
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite sua senha atual ou temporária.")]
        [DataType(DataType.Password)]
        public string SenhaProvisoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite a nova senha.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A nova senha deve ter no mínimo {2} caracteres.")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a nova senha.")]
        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação não batem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}