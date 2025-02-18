using FIAP.APIContato.Data;
using FIAP.APIContato.Events;
using FIAP.APIContato.Repositories;
using FIAP.APIContato.Services;
using FIAP.APIRegiao.Events;
using Microsoft.EntityFrameworkCore;
using Prometheus;

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
            builder.Services.AddDbContext<ContatosDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));
        }
        else
        {
            // Usar SQL Server em produção
            builder.Services.AddDbContext<ContatosDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }

        builder.Services.AddHttpClient(); // Adiciona a configuração do HttpClient
        builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
        builder.Services.AddScoped<IContatoService, ContatoService>();
        builder.Services.AddScoped<IRegiaoRepository, RegiaoAPIClient>();
        // Registro das configurações do RabbitMQ
        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
        // Registro do ContatoProducer como singleton para ser reutilizado
        builder.Services.AddSingleton<ContatoProducer>();

        // Serviço HTTP para Regiões
        builder.Services.AddHttpClient("ApiRegiao", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ApiRegiao:BaseUrl"] ?? "https://localhost:7294/api/");
        });

        builder.Services.AddControllers();

        // Configuração do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "API de Contatos",
                Version = "v1",
                Description = "API para gerenciamento de contatos"
            });
        });

        var app = builder.Build();

        // Chama o método para popular os dados
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var context = services.GetRequiredService<ContatosDbContext>();
        }

        // Configuração de Middlewares
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Contatos v1");
                options.RoutePrefix = "swagger";  // Abre na raiz do projeto
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        // Middleware de métricas do Prometheus
        app.UseHttpMetrics(); // Captura métricas HTTP automaticamente
        app.MapControllers();
        app.MapMetrics(); // Expondo métricas no endpoint /metrics

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}