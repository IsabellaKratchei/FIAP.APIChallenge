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

            // Configura��o de servi�os
            if (isTesting)
            {
                // Usar banco em mem�ria nos testes
                builder.Services.AddDbContext<RegiaoDbContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"));
            }
            else
            {
                // Usar SQL Server em produ��o
                builder.Services.AddDbContext<RegiaoDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();
            builder.Services.AddScoped<IRegiaoService, RegiaoService>();
            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
            builder.Services.AddSingleton<RegiaoProducer>();
            builder.Services.AddScoped<RegiaoConsumer>();

            builder.Services.AddControllers();

            // Configura��o do Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "API de Regi�es",
                    Version = "v1",
                    Description = "API para gerenciamento de regi�es com base no DDD"
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
                // Iniciar o consumidor de forma ass�ncrona sem bloquear o resto da execu��o
                _ = Task.Run(() => consumer.ConsumirMensagens());
                Console.WriteLine("Consumidor de mensagens iniciado com sucesso.");
                await PopulaRegioesSeVazioAsync(context, logger);
            }

            // Configura��o de Middlewares
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Regi�es v1");
                    options.RoutePrefix = "swagger";  // Abre na raiz do projeto
                });
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Middleware de m�tricas do Prometheus
            app.UseHttpMetrics(); // Captura m�tricas HTTP automaticamente
            app.MapControllers();
            app.MapMetrics(); // Expondo m�tricas no endpoint /metrics

            app.Run();
        }

        private static async Task PopulaRegioesSeVazioAsync(RegiaoDbContext context, ILogger logger)
        {
            // Verifica se estamos no ambiente de teste
            var isTestingEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing";

            // Verifica se j� existem registros na tabela de regi�es
            if (!await context.DDDs.AnyAsync())
            {
                // Defina o caminho do arquivo SQL para popular o banco de dados
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "ScriptDDD.sql");

                // Verifique se o arquivo de script existe
                if (File.Exists(scriptPath))
                {
                    logger.LogInformation("Executando script para popular tabela de regi�es...");

                    // Leia o conte�do do arquivo SQL
                    var scriptSql = await File.ReadAllTextAsync(scriptPath);

                    // Se estiver no ambiente de teste, executa no banco em mem�ria
                    if (isTestingEnvironment)
                    {
                        // Executa o script no banco em mem�ria
                        await context.Database.ExecuteSqlRawAsync(scriptSql);
                        logger.LogInformation("Tabela de regi�es populada com sucesso no banco em mem�ria.");
                    }
                    else
                    {
                        // Se for ambiente de produ��o, pode executar no banco de dados real
                        await context.Database.ExecuteSqlRawAsync(scriptSql);
                        logger.LogInformation("Tabela de regi�es populada com sucesso no banco de dados real.");
                    }
                }
                else
                {
                    logger.LogWarning($"Arquivo de script '{scriptPath}' n�o encontrado.");
                }
            }
            else
            {
                logger.LogInformation("Tabela de regi�es j� est� populada.");
            }
        }

    }
}
