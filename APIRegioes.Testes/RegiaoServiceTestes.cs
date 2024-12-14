using FIAP.APIRegiao.Models;
using FIAP.APIRegiao.Repository;
using FIAP.APIRegiao.Service;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace APIRegioes.Testes
{
    [TestFixture]
    public class RegiaoServiceTestes
    {
        private Mock<IRegiaoRepository> _mockRepository = null!;
        private RegiaoService _service = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRegiaoRepository>();
            _service = new RegiaoService(_mockRepository.Object);
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_ExistingDDD_ReturnsRegiao()
        {
            // Arrange
            var expectedRegiao = new RegiaoModel { Id = 1, DDD = "11", Regiao = "Sudeste" };
            _mockRepository.Setup(repo => repo.BuscarRegiaoPorDDDAsync("11")).ReturnsAsync(expectedRegiao);

            // Act
            var result = await _service.ObterRegiaoPorDDDAsync("11");

            // Assert
            Assert.That(result, Is.EqualTo(expectedRegiao));
        }

        [Test]
        public async Task BuscarRegiaoPorDDD_NonExistingDDD_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.BuscarRegiaoPorDDDAsync("999")).ReturnsAsync((RegiaoModel?)null);

            // Act
            var result = await _service.ObterRegiaoPorDDDAsync("999");

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}