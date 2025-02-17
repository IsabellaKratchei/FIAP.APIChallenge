using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using FIAP.APIRegiao.Repository;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace FIAP.APIRegiao.Events
{
    public class RegiaoConsumer
    {
        //private readonly RabbitMQSettings _settings;
        //private readonly IServiceScopeFactory _scopeFactory;  // Fábrica de escopos
        //private readonly IRegiaoService _regiaoService;  // Repositório de regiões

        //public RegiaoConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory)
        //{
        //    _settings = options.Value;
        //    _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        //}

        //public void ConsumirMensagens()
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = _settings.HostName,
        //        Port = 5672,  // Porta padrão do RabbitMQ
        //        UserName = "guest", // Usuário padrão do RabbitMQ
        //        Password = "guest"  // Senha padrão do RabbitMQ
        //    };

        //    // Criar conexão e canal
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        // Declarar a fila para garantir que ela existe
        //        channel.QueueDeclare(queue: _settings.QueueName,
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        // Criar um consumidor
        //        var consumer = new EventingBasicConsumer(channel);

        //        // Definir o que fazer quando uma mensagem for recebida
        //        consumer.Received += async (model, ea) =>
        //        {
        //            var body = ea.Body.ToArray();
        //            var mensagem = Encoding.UTF8.GetString(body);

        //            // Processar a mensagem (exemplo: atualizar região de um contato)
        //            await ProcessarMensagemAsync(mensagem);
        //        };

        //        // Consumir mensagens da fila
        //        channel.BasicConsume(queue: _settings.QueueName,
        //                             autoAck: true,  // Auto-acknowledgment
        //                             consumer: consumer);

        //        // Manter o processo em execução
        //        Console.WriteLine("Pressione [enter] para sair.");
        //        Console.ReadLine();
        //    }
        //}

        //private async Task ProcessarMensagemAsync(string mensagem)
        //{
        //    // Criar um escopo de serviço temporário
        //    using (var scope = _scopeFactory.CreateScope())
        //    {
        //        var regiaoService = scope.ServiceProvider.GetRequiredService<IRegiaoService>();

        //        // Aqui vai processar a mensagem de região recebida
        //        // Exemplo: buscar a região no repositório e atualizar algum contato

        //        Console.WriteLine($"Mensagem recebida: {mensagem}");

        //        // Extração do DDD da mensagem
        //        var ddd = ExtractDDDFromMessage(mensagem);

        //        if (!string.IsNullOrEmpty(ddd))
        //        {
        //            // Se a mensagem for um DDD, buscar a região
        //            var regiao = await regiaoService.ObterRegiaoPorDDDAsync(ddd);

        //            if (regiao != null)
        //            {
        //                // Atualizar a região de algum contato com base na mensagem
        //                // Aqui você pode fazer o que for necessário com o contato

        //                Console.WriteLine($"Região encontrada: {regiao.Regiao}");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Região não encontrada para o DDD: {ddd}");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("DDD não encontrado na mensagem.");
        //        }
        //    }
        //}

        //// Método auxiliar para extrair o DDD da mensagem
        //private string ExtractDDDFromMessage(string mensagem)
        //{
        //    // Usando expressão regular para capturar o DDD da mensagem
        //    var match = Regex.Match(mensagem, @"DDD:\s*(\d{2})");

        //    if (match.Success)
        //    {
        //        return match.Groups[1].Value;  // Retorna o DDD como string
        //    }
        //    return null;  // Se o DDD não for encontrado
        //}
    }
 }
