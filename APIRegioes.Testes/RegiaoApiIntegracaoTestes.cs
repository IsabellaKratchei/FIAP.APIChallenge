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

        // Defina o SetUp onde a injeção de dependência pode ser feita
        [SetUp]
        public void Setup()
        {
            // Defina a variável de ambiente para "Testing" antes de configurar o WebApplicationFactory
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            // Criação do Logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<RegiaoApiIntegracaoTestes>();

            // Configuração do WebApplicationFactory
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    var contentRoot = System.IO.Directory.GetCurrentDirectory();
                    _logger.LogInformation($"Configuring ContentRoot: {contentRoot}");
                    builder.UseContentRoot(contentRoot);

                    // Sobrescrever a configuração de DbContext para usar banco em memória
                    builder.ConfigureServices(services =>
                    {
                        // Remove a configuração existente do DbContext
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RegiaoDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Adiciona a configuração do DbContext para usar o banco em memória
                        services.AddDbContext<RegiaoDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDatabase"); // Banco em memória
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
            // Arrange: Adiciona dados ao banco em memória
            var ddd = "41"; // Substitua por um DDD válido para teste
            var dbContext = _factory.Services.GetRequiredService<RegiaoDbContext>();
            dbContext.DDDs.Add(new RegiaoModel { Id = 1, DDD = "41", Regiao = "Sul" });
            await dbContext.SaveChangesAsync();

            var url = $"/api/Regiao/{ddd}";

            // Act: Faz a requisição à API
            var response = await _client.GetAsync(url);

            // Assert: Verifica se a resposta é bem-sucedida
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_InvalidDDD_ReturnsNotFound()
        {
            // Arrange: DDD inexistente
            var ddd = "999";
            var url = $"/api/Regiao/{ddd}";

            // Act: Faz a requisição à API
            var response = await _client.GetAsync(url);

            // Assert: Verifica se o status retornado é NotFound
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }
    }
}
