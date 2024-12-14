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

            // Configura��o de servi�os
            builder.Services.AddDbContext<RegiaoDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRegiaoRepository, RegiaoRepository>();
            builder.Services.AddScoped<IRegiaoService, RegiaoService>();

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

            var app = builder.Build();
            // Chama o m�todo para popular os dados
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<RegiaoDbContext>();
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
            // Verifica se j� existem registros na tabela de regi�es
            if (!await context.DDDs.AnyAsync())
            {
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "ScriptRegioes.sql");
                if (File.Exists(scriptPath))
                {
                    logger.LogInformation("Executando script para popular tabela de regi�es...");

                    var scriptSql = await File.ReadAllTextAsync(scriptPath);
                    await context.Database.ExecuteSqlRawAsync(scriptSql);

                    logger.LogInformation("Tabela de regi�es populada com sucesso.");
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
