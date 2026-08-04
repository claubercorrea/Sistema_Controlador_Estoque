using ClosedXML.Excel;
using SistemaCompleto.TDO;

namespace SistemaCompleto.Services
{
    public interface IExcelService
    {
        public Task<byte[]> ExportToExcelAsync(IEnumerable<ProdutoDto> produtos);
        public byte[] GerarExcel(IEnumerable<ProdutoDto> produtos);
    }


    public class ExcelService : IExcelService
    {
        public async Task<byte[]> ExportToExcelAsync(IEnumerable<ProdutoDto> produtos)
        {
            return await Task.Run(() => GerarExcel(produtos));
        }

        public byte[] GerarExcel(IEnumerable<ProdutoDto> produtos)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Produtos");
             
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Nome";
                worksheet.Cell(1, 3).Value = "Preço";
                worksheet.Cell(1, 4).Value = "Validade";
                worksheet.Cell(1, 5).Value = "Quantidade em Estoque";
    
                int row = 2;
                foreach (var produto in produtos)
                {
                    worksheet.Cell(row, 1).Value = produto.Id;
                    worksheet.Cell(row, 2).Value = produto.Nome;
                    worksheet.Cell(row, 3).Value = produto.Preco;
                    worksheet.Cell(row, 4).Value = produto.Validade.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 5).Value = produto.QuantidadeEstoque;
                    row++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}


