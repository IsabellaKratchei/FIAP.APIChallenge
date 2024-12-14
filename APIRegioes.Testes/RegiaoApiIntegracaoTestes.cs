using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using FIAP.APIRegiao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FIAP.APIRegiao.Models;

namespace APIRegioes.Testes
{
    [TestFixture]
    public class RegiaoApiIntegracaoTestes
    {
        private HttpClient _client = null!;
        private WebApplicationFactory<Program> _factory = null!;
        private ILogger<RegiaoApiIntegracaoTestes> _logger = null!;

        // Defina o SetUp onde a inje��o de depend�ncia pode ser feita
        [SetUp]
        public void Setup()
        {
            // Defina a vari�vel de ambiente para "Testing" antes de configurar o WebApplicationFactory
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            // Cria��o do Logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<RegiaoApiIntegracaoTestes>();

            // Configura��o do WebApplicationFactory
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    var contentRoot = System.IO.Directory.GetCurrentDirectory();
                    _logger.LogInformation($"Configuring ContentRoot: {contentRoot}");
                    builder.UseContentRoot(contentRoot);

                    // Sobrescrever a configura��o de DbContext para usar banco em mem�ria
                    builder.ConfigureServices(services =>
                    {
                        // Remove a configura��o existente do DbContext
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RegiaoDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Adiciona a configura��o do DbContext para usar o banco em mem�ria
                        services.AddDbContext<RegiaoDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDatabase"); // Banco em mem�ria
                        });

                        // Certifique-se de que a base de dados seja criada antes dos testes
                        var serviceProvider = services.BuildServiceProvider();
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            var dbContext = scopedServices.GetRequiredService<RegiaoDbContext>();
                            dbContext.Database.EnsureCreated();
                        }
                    });
                });

            _client = _factory.CreateClient();
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_ValidDDD_ReturnsSuccessStatusCode()
        {
            // Arrange: Adiciona dados ao banco em mem�ria
            var ddd = "41"; // Substitua por um DDD v�lido para teste
            var dbContext = _factory.Services.GetRequiredService<RegiaoDbContext>();
            dbContext.DDDs.Add(new RegiaoModel { Id = 1, DDD = "41", Regiao = "Sul" });
            await dbContext.SaveChangesAsync();

            var url = $"/api/Regiao/{ddd}";

            // Act: Faz a requisi��o � API
            var response = await _client.GetAsync(url);

            // Assert: Verifica se a resposta � bem-sucedida
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_InvalidDDD_ReturnsNotFound()
        {
            // Arrange: DDD inexistente
            var ddd = "999";
            var url = $"/api/Regiao/{ddd}";

            // Act: Faz a requisi��o � API
            var response = await _client.GetAsync(url);

            // Assert: Verifica se o status retornado � NotFound
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }
    }
}
