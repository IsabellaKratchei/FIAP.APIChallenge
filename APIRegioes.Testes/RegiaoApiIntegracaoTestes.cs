using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using FIAP.APIRegiao;

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
                });

            _client = _factory.CreateClient();
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_ValidDDD_ReturnsSuccessStatusCode()
        {
            // Arrange
            var ddd = "41"; // Substitua por um DDD existente na sua base de testes
            var url = $"/api/Regiao/{ddd}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_InvalidDDD_ReturnsNotFound()
        {
            // Arrange
            var ddd = "999"; // DDD inexistente
            var url = $"/api/Regiao/{ddd}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }
    }
}
