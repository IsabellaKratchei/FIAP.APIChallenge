using FIAP.APIRegiao.Events;
using FIAP.APIRegiao.Repository;
using FIAP.APIRegiao.Service;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace FIAP.APIRegiao
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Verifica se estamos em ambiente de testes
            var isTesting = builder.Environment.IsEnvironment("Testing");

            // Configuração de serviços
            if (isTesting)
            {
                // Usar banco em memória nos testes
                builder.Services.AddDbContext<RegiaoDbContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"));
            }
            else
            {
                // Usar SQL Server em produção
                builder.Services.AddDbContext<RegiaoDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();
            builder.Services.AddScoped<IRegiaoService, RegiaoService>();
            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
            builder.Services.AddSingleton<RegiaoProducer>();
            builder.Services.AddScoped<RegiaoConsumer>();

            builder.Services.AddControllers();

            // Configuração do Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "API de Regiões",
                    Version = "v1",
                    Description = "API para gerenciamento de regiões com base no DDD"
                });
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<RegiaoDbContext>();
                var consumer = scope.ServiceProvider.GetRequiredService<RegiaoConsumer>();
                // Iniciar o consumidor de forma assíncrona sem bloquear o resto da execução
                _ = Task.Run(() => consumer.ConsumirMensagens());
                Console.WriteLine("Consumidor de mensagens iniciado com sucesso.");
                await PopulaRegioesSeVazioAsync(context, logger);
            }

            // Configuração de Middlewares
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Regiões v1");
                    options.RoutePrefix = "swagger";  // Abre na raiz do projeto
                });
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Middleware de métricas do Prometheus
            app.UseHttpMetrics(); // Captura métricas HTTP automaticamente
            app.MapControllers();
            app.MapMetrics(); // Expondo métricas no endpoint /metrics

            app.Run();
        }

        private static async Task PopulaRegioesSeVazioAsync(RegiaoDbContext context, ILogger logger)
        {
            // Verifica se estamos no ambiente de teste
            var isTestingEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing";

            // Verifica se já existem registros na tabela de regiões
            if (!await context.DDDs.AnyAsync())
            {
                // Defina o caminho do arquivo SQL para popular o banco de dados
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "ScriptDDD.sql");

                // Verifique se o arquivo de script existe
                if (File.Exists(scriptPath))
                {
                    logger.LogInformation("Executando script para popular tabela de regiões...");

                    // Leia o conteúdo do arquivo SQL
                    var scriptSql = await File.ReadAllTextAsync(scriptPath);

                    // Se estiver no ambiente de teste, executa no banco em memória
                    if (isTestingEnvironment)
                    {
                        // Executa o script no banco em memória
                        await context.Database.ExecuteSqlRawAsync(scriptSql);
                        logger.LogInformation("Tabela de regiões populada com sucesso no banco em memória.");
                    }
                    else
                    {
                        // Se for ambiente de produção, pode executar no banco de dados real
                        await context.Database.ExecuteSqlRawAsync(scriptSql);
                        logger.LogInformation("Tabela de regiões populada com sucesso no banco de dados real.");
                    }
                }
                else
                {
                    logger.LogWarning($"Arquivo de script '{scriptPath}' não encontrado.");
                }
            }
            else
            {
                logger.LogInformation("Tabela de regiões já está populada.");
            }
        }

    }
}
