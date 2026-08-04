public interface IProdutoServicecs
{
    Task<IEnumerable<ProdutoDto>> GetAllProdutosAsync();
    Task<ProdutoDto?> GetProdutoByIdAsync(int id);
    Task CreateProdutoAsync(ProdutoDto produtoDto);
    Task UpdateProdutoAsync(ProdutoDto produtoDto);
    Task DeleteProdutoAsync(int id);
    Task<bool> CodigoBarraExisteAsync(int codigoBarra); // Adicionado para corrigir CS1061
}