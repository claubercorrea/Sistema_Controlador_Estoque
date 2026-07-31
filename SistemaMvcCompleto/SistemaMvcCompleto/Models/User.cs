using Microsoft.AspNetCore.Identity;

namespace SistemaCompleto.Models
{
    public class User : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        
        // 🔒 Define que por padrão todo usuário novo precisará alterar a senha
        public bool PrimeiroAcesso { get; set; } = true;
    }
}