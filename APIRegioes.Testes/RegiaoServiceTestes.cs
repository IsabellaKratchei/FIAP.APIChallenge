using FIAP.APIRegiao.Models;
using FIAP.APIRegiao.Repository;
using FIAP.APIRegiao.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace APIRegioes.Testes
{
    [TestFixture]
    public class RegiaoServiceTestes
    {
        private RegiaoDbContext _dbContext = null!;
        private RegiaoService _service = null!;

        [SetUp]
        public void Setup()
        {
            // Defina a variável de ambiente para "Testing" antes de configurar o WebApplicationFactory
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<RegiaoDbContext>()
                .UseInMemoryDatabase("TestDatabase") // Banco único para os testes
                .Options;

            // Inicializa o contexto com o banco em memória
            _dbContext = new RegiaoDbContext(options);

            // Limpa o banco de dados antes de cada teste
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            // Inicializa o repositório e o serviço
            var repository = new RegiaoRepository(_dbContext);
            _service = new RegiaoService(repository);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_ExistingDDD_ReturnsRegiao()
        {
            // Arrange: Popula o banco em memória com um registro
            var expectedRegiao = new RegiaoModel { Id = 1, DDD = "11", Regiao = "Sudeste" };
            _dbContext.DDDs.Add(expectedRegiao);
            await _dbContext.SaveChangesAsync();

            // Act: Chama o método do serviço
            var result = await _service.ObterRegiaoPorDDDAsync("11");

            // Assert: Verifica se o retorno é o esperado
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DDD, Is.EqualTo(expectedRegiao.DDD));
            Assert.That(result.Regiao, Is.EqualTo(expectedRegiao.Regiao));
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_NonExistingDDD_ReturnsNull()
        {
            // Arrange: Banco em memória vazio

            // Act: Chama o método do serviço para um DDD que não existe
            var result = await _service.ObterRegiaoPorDDDAsync("999");

            // Assert: Verifica se o retorno é nulo
            Assert.That(result, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            // Limpa o banco de dados em memória entre testes
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}