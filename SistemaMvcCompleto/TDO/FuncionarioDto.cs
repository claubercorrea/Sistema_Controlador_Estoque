using System.ComponentModel.DataAnnotations;

namespace SistemaCompleto.TDO
{
    public class FuncionarioDto
    {
        public static object Cargo { get; internal set; }
        public string Id { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CargoRole { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public bool IsTemporario { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class EditarPermissoesDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione o nível de permissão.")]
        [Display(Name = "Nível de Permissão (Role)")]
        public string TipoAcesso { get; set; } = "Usuario"; 

        [Display(Name = "Data de Expiração (Uso Temporário)")]
        [DataType(DataType.Date)]
        public DateTime? DataExpiracao { get; set; }
    }
}
