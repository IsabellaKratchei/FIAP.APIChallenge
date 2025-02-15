using FIAP.APIContato.Models;

namespace FIAP.APIContato.Repositories
{
    public interface IContatoRepository
    {
        Task<ContatoModel> BuscarPorIdAsync(int id);
        Task<List<ContatoModel>> BuscarTodosAsync();
        Task<ContatoModel> AdicionarAsync(ContatoModel contato);
        Task<ContatoModel> EditarAsync(ContatoModel contato);
        Task<bool> ApagarAsync(int id);
        Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd);

        // Novo método para publicar eventos no RabbitMQ
        //void PublicarEventoNoRabbitMQ(string evento, ContatoModel contato);
    }
}