namespace SistemaCompleto.TDO
{
    public class UsuarioViewModel
    {
      
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string NomeCompleto { get; set; } = string.Empty;
            public IList<string> Roles { get; set; } = new List<string>();
            public bool PrimeiroAcesso { get; set; } = true;

    }
    }

