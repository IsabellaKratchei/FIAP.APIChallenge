using FIAP.APIContato.Models;
using FIAP.APIContato.Repositories;

namespace FIAP.APIContato.Services
{
    public class ContatoService : IContatoService
    {
        private readonly IContatoRepository _contatoRepository;

        public ContatoService(IContatoRepository contatoRepository)
        {
            _contatoRepository = contatoRepository;
        }

        public async Task<ContatoModel> BuscarPorIdAsync(int id)
        {
            return await _contatoRepository.BuscarPorIdAsync(id);
        }

        public async Task<List<ContatoModel>> BuscarTodosAsync()
        {
            return await _contatoRepository.BuscarTodosAsync();
        }

        public async Task<ContatoModel> AdicionarAsync(ContatoModel contato)
        {
            try
            {
                var novoContato = await _contatoRepository.AdicionarAsync(contato);

                // Publicar evento no RabbitMQ para criação do contato
                _contatoRepository.PublicarEventoNoRabbitMQ("ContatoCriado", novoContato);

                return novoContato;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar contato: {ex.Message}");
            }
        }

        public async Task<ContatoModel> EditarAsync(ContatoModel contato)
        {
            try
            {
                var contatoEditado = await _contatoRepository.EditarAsync(contato);

                // Publicar evento no RabbitMQ para edição do contato
                _contatoRepository.PublicarEventoNoRabbitMQ("ContatoEditado", contatoEditado);

                return contatoEditado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao editar contato: {ex.Message}");
            }
        }

        public async Task<bool> ApagarAsync(int id)
        {
            try
            {
                var sucesso = await _contatoRepository.ApagarAsync(id);

                // Publicar evento no RabbitMQ para exclusão do contato
                if (sucesso)
                {
                    _contatoRepository.PublicarEventoNoRabbitMQ("ContatoExcluido", new ContatoModel { Id = id });
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir contato: {ex.Message}");
            }
        }

        public async Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd)
        {
            return await _contatoRepository.BuscarPorDDDAsync(ddd);
        }
    }
}
