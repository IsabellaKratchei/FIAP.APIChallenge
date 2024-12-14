using FIAP.APIRegiao.Repository;
using FIAP.APIRegiao.Service;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIRegiao
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuração de serviços
            builder.Services.AddDbContext<RegiaoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();
            builder.Services.AddScoped<IRegiaoService, RegiaoService>();

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

            var app = builder.Build();
            // Chama o método para popular os dados
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<RegiaoDbContext>();
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
                    options.RoutePrefix = string.Empty;  // Abre na raiz do projeto
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static async Task PopulaRegioesSeVazioAsync(RegiaoDbContext context, ILogger logger)
        {
            // Verifica se já existem registros na tabela de regiões
            if (!await context.DDDs.AnyAsync())
            {
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "ScriptRegioes.sql");
                if (File.Exists(scriptPath))
                {
                    logger.LogInformation("Executando script para popular tabela de regiões...");

                    var scriptSql = await File.ReadAllTextAsync(scriptPath);
                    await context.Database.ExecuteSqlRawAsync(scriptSql);

                    logger.LogInformation("Tabela de regiões populada com sucesso.");
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
