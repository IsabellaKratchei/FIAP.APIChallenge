using FIAP.APIContato.Models;
using FIAP.APIContato.Repositories;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIContato.Services
{
    public class ContatoService : IContatoService
    {
        private readonly IContatoRepository _contatoRepository;
        private readonly IRegiaoRepository _regiaoRepository;

        public ContatoService(IContatoRepository contatoRepository, IRegiaoRepository regiaoRepository)
        {
            _contatoRepository = contatoRepository;
            _regiaoRepository = regiaoRepository; // Injetando o repositório de região
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
                // Obter a região baseada no DDD
                var regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);
                if (regiao == null)
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
                }

                // Atribuir a região ao contato
                contato.Regiao = regiao.Regiao;

                var novoContato = await _contatoRepository.AdicionarAsync(contato);

                // Publicar evento no RabbitMQ para criação do contato
                //_contatoRepository.PublicarEventoNoRabbitMQ("ContatoCriado", novoContato);

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
                // Obter a região baseada no DDD
                var regiao = await _regiaoRepository.BuscarRegiaoPorDDDAsync(contato.DDD);
                if (regiao == null)
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
                }

                // Atribuir a região ao contato
                contato.Regiao = regiao.Regiao;

                var contatoEditado = await _contatoRepository.EditarAsync(contato);

                // Publicar evento no RabbitMQ para edição do contato
                //_contatoRepository.PublicarEventoNoRabbitMQ("ContatoEditado", contatoEditado);

                return contatoEditado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao editar contato: {ex.Message}");
            }
        }

        public async Task<bool> ApagarAsync(int id)
        {
            var contato = await _contatoRepository.BuscarPorIdAsync(id);
            if (contato == null)
            {
                return false; // Contato não encontrado
            }

            await _contatoRepository.ApagarAsync(contato.Id);
            return true; // Exclusão bem-sucedida
        }

        public async Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd)
        {
            return await _contatoRepository.BuscarPorDDDAsync(ddd);
        }

        // Publicar eventos no RabbitMQ
        private void PublicarMensagemNoRabbitMQ(string evento, ContatoModel contato)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "contato_eventos",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var mensagem = JsonSerializer.Serialize(new { Evento = evento, Contato = contato });
                var body = Encoding.UTF8.GetBytes(mensagem);

                channel.BasicPublish(exchange: "",
                                     routingKey: "contato_eventos",
                                     basicProperties: null,
                                     body: body);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao publicar mensagem no RabbitMQ: " + ex.Message);
            }
        }
    }
}
