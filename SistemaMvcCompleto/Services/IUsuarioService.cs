using Microsoft.AspNetCore.Identity;

namespace SistemaMvcCompleto.Services
{
    public interface IUsuarioService
    {
        Task<IdentityResult> ExcluirUsuarioAsync(string id);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsuarioService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ExcluirUsuarioAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            return await _userManager.DeleteAsync(usuario);
        }
    }
}
