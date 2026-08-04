using SistemaCompleto.Models;

namespace SistemaCompleto.Services
{
    public interface ITokenService
    {
        Task<string> GerarTokenAsync(User user);
       
    }
}