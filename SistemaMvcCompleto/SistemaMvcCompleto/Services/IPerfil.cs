//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using SistemaCompleto.Data;
//using SistemaCompleto.Models;
//using SistemaCompleto.TDO;
//using SistemaMvcCompleto.Services;
//using SistemaMvcCompleto.TDO;

//namespace SistemaMvcCompleto.Services
//{
//    public interface IPerfil
//    {

//        Task<PerfilDto?> GetPerfilByIdAsync(PerfiUser user);
//    }

//    public class PerfilFotoService : IPerfil
//    {

//        private readonly UserManager<User> _userManager;
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        // 💉 Injetamos o gerenciador de usuários e o ambiente web no construtor
//        public PerfilFotoService(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
//        {
//            _userManager = userManager;
//            _webHostEnvironment = webHostEnvironment;
//        }

//        public async Task<User> AtulizarPerfilByAsync(PerfilDto User)
//        {
//            var usuario = await _userManager.GetUserAsync(User);
//            if (usuario == null)
//            {
//                // Corrigido: lançar exceção ou retornar null, pois NotFound não existe fora de controllers
//                throw new InvalidOperationException("Usuário não encontrado.");
//                // ou, alternativamente:
//                // return null;
//            }

//            // 🛑 2. Verifica se o modelo enviado é válido
//            // ModelState também não existe fora de controllers, remova ou implemente validação manual
//            if (!ModelState.IsValid)
//             {
//            //     return View("Index", model);
//            }

//            // Implemente o restante da lógica aqui
//            return usuario;
//        }
//    }
//}


 