using FIAP.APIContato.Models;

namespace FIAP.APIContato.Repositories
{
    public interface IContatoService
    {
        Task<ContatoModel> BuscarPorIdAsync(int id);
        Task<List<ContatoModel>> BuscarTodosAsync();
        Task<ContatoModel> AdicionarAsync(ContatoModel contato);
        Task<ContatoModel> EditarAsync(ContatoModel contato);
        Task<bool> ApagarAsync(int id);
        Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd);
    }
}