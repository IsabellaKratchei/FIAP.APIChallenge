//using RabbitMQ.Client.Events;
//using RabbitMQ.Client;
//using System.Text;
//using FIAP.APIRegiao.Repository;
//using Microsoft.Extensions.Options;
//using System.Text.RegularExpressions;

//namespace FIAP.APIRegiao.Events
//{
//    public class RegiaoConsumer : BackgroundService
//    {
//        private readonly RabbitMQSettings _settings;
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly ILogger<RegiaoConsumer> _logger;

//        public RegiaoConsumer(IOptions<RabbitMQSettings> options,
//                              IServiceScopeFactory scopeFactory,
//                              ILogger<RegiaoConsumer> logger)
//        {
//            _settings = options.Value;
//            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
//            _logger = logger;
//        }

//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var factory = new ConnectionFactory()
//            {
//                HostName = _settings.HostName,
//                Port = 5672//,  // Porta padrão do RabbitMQ
//                //UserName = _settings.UserName,
//                //Password = _settings.Password
//            };

//            var connection = factory.CreateConnection();
//            var channel = connection.CreateModel();

//            // Declarar a fila para garantir que ela existe
//            channel.QueueDeclare(queue: _settings.QueueName,
//                                 durable: false,
//                                 exclusive: false,
//                                 autoDelete: false,
//                                 arguments: null);

//            var consumer = new EventingBasicConsumer(channel);

//            consumer.Received += async (model, ea) =>
//            {
//                var body = ea.Body.ToArray();
//                var mensagem = Encoding.UTF8.GetString(body);

//                _logger.LogInformation($"Mensagem recebida: {mensagem}");

//                // Processa a mensagem recebida
//                await ProcessarMensagemAsync(mensagem);

//                // Acknowledge manual da mensagem
//                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//            };

//            channel.BasicConsume(queue: _settings.QueueName,
//                                 autoAck: false,
//                                 consumer: consumer);

//            // Registra a ação de encerramento do serviço para fechar a conexão e o canal
//            stoppingToken.Register(() =>
//            {
//                _logger.LogInformation("Encerrando conexão com RabbitMQ...");
//                channel.Close();
//                connection.Close();
//            });

//            return Task.CompletedTask;
//        }

//        private async Task ProcessarMensagemAsync(string mensagem)
//        {
//            using (var scope = _scopeFactory.CreateScope())
//            {
//                var regiaoService = scope.ServiceProvider.GetRequiredService<IRegiaoService>();

//                _logger.LogInformation($"Processando mensagem: {mensagem}");

//                // Extrair o DDD da mensagem
//                var ddd = ExtractDDDFromMessage(mensagem);

//                if (!string.IsNullOrEmpty(ddd))
//                {
//                    var regiao = await regiaoService.ObterRegiaoPorDDDAsync(ddd);

//                    if (regiao != null)
//                    {
//                        _logger.LogInformation($"Região encontrada para DDD {ddd}: {regiao.Regiao}");
//                        // Aqui você pode implementar ações adicionais, como atualizar algum registro ou notificar outros serviços
//                    }
//                    else
//                    {
//                        _logger.LogWarning($"Região não encontrada para o DDD: {ddd}");
//                    }
//                }
//                else
//                {
//                    _logger.LogWarning("DDD não encontrado na mensagem.");
//                }
//            }
//        }

//        // Método auxiliar para extrair o DDD da mensagem usando Regex
//        private string ExtractDDDFromMessage(string mensagem)
//        {
//            var match = Regex.Match(mensagem, @"DDD:\s*(\d{2})");
//            return match.Success ? match.Groups[1].Value : null;
//        }
//    }
//}
