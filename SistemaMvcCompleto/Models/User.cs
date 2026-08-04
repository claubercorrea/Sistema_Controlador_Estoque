using Microsoft.AspNetCore.Identity;

namespace SistemaCompleto.Models
{
    public class User : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        
        public bool PrimeiroAcesso { get; set; } = true;
    }
}