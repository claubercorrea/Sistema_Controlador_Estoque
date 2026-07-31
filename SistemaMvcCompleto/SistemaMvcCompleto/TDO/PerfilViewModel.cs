//namespace CadastroProdutos.Models
//{
//    public class PerfilViewModel
//    {
//        //  Permite que o usuário também atualize o nome se quiser
//        public string NomeCompleto { get; set; } = string.Empty;

//        //  Propriedade que vai receber o arquivo da foto vindo do formulário
//        public IFormFile? Foto { get; set; }
//    }
//}
using Microsoft.AspNetCore.Http;

namespace CadastroProdutos.Models
{
    public class PerfilViewModel
    {
        public string NomeCompleto { get; set; } 

        // Propriedades úteis caso queira exibir na View de Perfil
        public string? Email { get; set; }
        public string? FotoUrlAtual { get; set; }

        // Propriedade que vai receber o arquivo da foto vindo do formulário
        public IFormFile? Foto { get; set; }
    }
}