using FIAP.APIContato.Events;
using FIAP.APIContato.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FIAP.APIRegiao.Events
{
    public class ContatoProducer
    {
        private readonly RabbitMQSettings _settings;

        public ContatoProducer(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Publica um evento com o tipo especificado e o payload do contato.
        /// </summary>
        /// <param name="evento">Tipo do evento (ex: "ContatoCriado", "ContatoAtualizado")</param>
        /// <param name="contato">Objeto ContatoModel que será serializado</param>
        public void PublicarMensagem(string evento, ContatoModel contato)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declarar uma exchange do tipo "topic" para eventos de contato
            string exchangeName = "contato.events";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

            // Cria um payload que inclui o tipo do evento e os dados
            var payload = new
            {
                Evento = evento,
                Data = contato
            };

            var mensagem = JsonSerializer.Serialize(payload);
            var body = Encoding.UTF8.GetBytes(mensagem);

            // Define a routing key com base no tipo de evento
            string routingKey = evento.ToLower(); // ex: "contatocriado", "contatoatualizado"

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
        }

        public void PublicarSolicitandoRegiao(string evento, string ddd)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declarar a exchange (se ainda não estiver declarada)
            string exchangeName = "contato.events";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

            // Serializa o payload (nesse caso, apenas o DDD, mas você pode usar JSON se desejar mais dados)
            var body = Encoding.UTF8.GetBytes(ddd);

            // A routing key para solicitações de região – use um nome padrão, por exemplo "solicitandoregiao"
            string routingKey = "SolicitandoRegiao";

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
        }
    }
}
