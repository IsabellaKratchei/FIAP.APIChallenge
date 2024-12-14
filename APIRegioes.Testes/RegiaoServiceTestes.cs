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
            // For�a o uso do banco em mem�ria
            var options = new DbContextOptionsBuilder<RegiaoDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Inicializa o contexto e o servi�o
            _dbContext = new RegiaoDbContext(options);
            var repository = new RegiaoRepository(_dbContext);
            _service = new RegiaoService(repository);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_ExistingDDD_ReturnsRegiao()
        {
            // Arrange: Popula o banco em mem�ria com um registro
            var expectedRegiao = new RegiaoModel { Id = 1, DDD = "11", Regiao = "Sudeste" };
            _dbContext.DDDs.Add(expectedRegiao);
            await _dbContext.SaveChangesAsync();

            // Act: Chama o m�todo do servi�o
            var result = await _service.ObterRegiaoPorDDDAsync("11");

            // Assert: Verifica se o retorno � o esperado
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DDD, Is.EqualTo(expectedRegiao.DDD));
            Assert.That(result.Regiao, Is.EqualTo(expectedRegiao.Regiao));
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_NonExistingDDD_ReturnsNull()
        {
            // Arrange: Banco em mem�ria vazio

            // Act: Chama o m�todo do servi�o para um DDD que n�o existe
            var result = await _service.ObterRegiaoPorDDDAsync("999");

            // Assert: Verifica se o retorno � nulo
            Assert.That(result, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            // Limpa o banco de dados em mem�ria entre testes
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}