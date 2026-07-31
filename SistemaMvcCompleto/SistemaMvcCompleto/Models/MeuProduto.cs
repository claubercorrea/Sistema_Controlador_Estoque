using System.ComponentModel.DataAnnotations;
using SistemaCompleto.Models;

namespace SistemaCompleto.Models
{
    public class MeuProduto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome do Produto é obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Nome do Produto")]
        public string Nome_Produto { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo categoria é obrigatório.")]
        [StringLength(100)]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo quantidade é obrigatório.")]
        [Display(Name = "Quantidade")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número inteiro não negativo.")]
        public int Quantidade { get; set; }
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, 999999.99, ErrorMessage = "Informe um preço válido.")]
        public decimal Preco { get; set; }
        [Required(ErrorMessage = "O campo validade é obrigatório.")]
        [Display(Name = "Validade")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
   
        public DateTime Validade { get; set; }
        [Required(ErrorMessage = "O campo codigo de barra é obrigatório.")]
        [Display(Name = "Código de Barra")]
        [Range(0, int.MaxValue, ErrorMessage = "O código de barra deve ser um número inteiro não negativo.")]
        public int Codigo_barra { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
