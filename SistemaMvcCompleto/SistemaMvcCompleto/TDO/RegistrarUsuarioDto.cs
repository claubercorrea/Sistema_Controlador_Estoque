using System.ComponentModel.DataAnnotations;

namespace SistemaMvcCompleto.TDO
{
    public class RegistrarUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo {2} caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        // 🛡️ Nível de Permissão definido pelo Admin
        [Required(ErrorMessage = "Selecione o tipo de acesso.")]
        [Display(Name = "Tipo de Acesso / Nível de Permissão")]
        public string TipoAcesso { get; set; } = "Usuario"; // Opções: "Admin", "Usuario", "Temporario"

        // ⏳ Validade para Acesso Temporário
        [Display(Name = "Data de Expiração (Apenas para Acesso Temporário)")]
        [DataType(DataType.Date)]
        public DateTime? DataExpiracao { get; set; }
    }
}
