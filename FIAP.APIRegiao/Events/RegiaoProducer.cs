using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Net;
using System.Text;

namespace FIAP.APIRegiao.Events
{
    public class RegiaoProducer
    {
        private readonly RabbitMQSettings _settings;

        public RegiaoProducer(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
        }

        public void PublicarMensagem(string mensagem)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = 5672,  // Porta padrão do RabbitMQ
                UserName = "guest", // Usuário padrão do RabbitMQ
                Password = "guest"  // Senha padrão do RabbitMQ
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _settings.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(mensagem);

            channel.BasicPublish(exchange: "",
                                 routingKey: _settings.QueueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
