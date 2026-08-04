using System.ComponentModel.DataAnnotations;

namespace SistemaCompleto.TDO
{
    public class ProdutoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço é obrigatório.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public int QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "A validade é obrigatória.")]
        public DateTime Validade { get; set; }

        [Required(ErrorMessage = "O código de barras é obrigatório.")]
        public int Codigo_barra { get; set; }
    }
}