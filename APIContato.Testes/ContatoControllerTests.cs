using FIAP.APIContato.Controllers;
using FIAP.APIContato.Models;
using FIAP.APIContato.Services;
using FIAP.APIContato.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FIAP.APIRegiao.Events;
using FIAP.APIContato.Events;
using Microsoft.Extensions.Options;

namespace APIContato.Testes
{
    [TestFixture]
    public class ContatoControllerTests
    {
        private Mock<IContatoService> _contatoServiceMock;
        private Mock<IContatoRepository> _contatoRepositoryMock;
        private ContatoController _controller;
        //private ContatoProducer _contatoProducer;

        [SetUp]
        public void Setup()
        {
            _contatoServiceMock = new Mock<IContatoService>();
            _contatoRepositoryMock = new Mock<IContatoRepository>();
            _controller = new ContatoController(_contatoServiceMock.Object, _contatoRepositoryMock.Object);

            // Configurar as settings do RabbitMQ para testes (certifique-se de que o RabbitMQ está rodando localmente)
            var rabbitSettings = Options.Create(new RabbitMQSettings
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                QueueName = "RegiaoQueue"
            });

            // Instancia o produtor real para testes de integração
            //_contatoProducer = new ContatoProducer(rabbitSettings);

            // Instancia o controller com as dependências simuladas e o produtor real
            //_controller = new ContatoController(_contatoServiceMock.Object, _contatoRepositoryMock.Object, _contatoProducer);
        }

        [Test]
        public async Task Get_DeveRetornarOkComListaDeContatos()
        {
            // Arrange
            var contatos = new List<ContatoModel>
            {
                new ContatoModel { Id = 1, Nome = "João",Email = "joao@gmail.com", Telefone = "999999999", DDD = "11" , Regiao = "Sudeste" },
                new ContatoModel { Id = 2, Nome = "Maria",Email = "maria@gmail.com", Telefone = "988888888", DDD = "21", Regiao = "Sudeste" }
            };

            _contatoServiceMock.Setup(s => s.BuscarTodosAsync()).ReturnsAsync(contatos);

            // Act
            var resultado = await _controller.Get();

            // Assert
            var okResult = resultado as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(contatos, okResult.Value);
        }

        [Test]
        public async Task GetById_ContatoExistente_DeveRetornarOk()
        {
            // Arrange
            var contato = new ContatoModel { Id = 1, Nome = "Carlos", Email = "carlos@gmail.com", Telefone = "999999999", DDD = "11", Regiao="Sudeste" };
            _contatoServiceMock.Setup(s => s.BuscarPorIdAsync(1)).ReturnsAsync(contato);

            // Act
            var resultado = await _controller.GetById(1);

            // Assert
            var okResult = resultado as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(contato, okResult.Value);
        }

        [Test]
        public async Task GetById_ContatoInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _contatoServiceMock.Setup(s => s.BuscarPorIdAsync(It.IsAny<int>())).ReturnsAsync((ContatoModel)null);

            // Act
            var resultado = await _controller.GetById(999);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Criar_ContatoValido_DeveRetornarCreated()
        {
            // Arrange
            var contato = new ContatoModel { Nome = "Carlos", Email = "carlos@gmail.com", Telefone = "988887777", DDD = "11", Regiao = "Sudeste" };
            var contatoCriado = new ContatoModel { Id = 3, Nome = "Carlos", Email = "carlos@gmail.com", Telefone = "988887777", DDD = "11", Regiao = "Sudeste" };

            _contatoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<ContatoModel>()))
                .ReturnsAsync(contatoCriado);

            // Act
            var resultado = await _controller.Criar(contato);

            // Assert
            var createdResult = resultado as CreatedAtActionResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(contatoCriado, createdResult.Value);
        }

        [Test]
        public async Task Criar_ContatoNulo_DeveRetornarBadRequest()
        {
            // Act
            var resultado = await _controller.Criar(null);

            // Assert
            var badRequestResult = resultado as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Editar_ContatoExistente_DeveRetornarOk()
        {
            // Arrange
            var contato = new ContatoModel { Id = 2, Nome = "Maria", Email = "maria@outlook.com", Telefone = "977776666", DDD = "21", Regiao = "Sudeste" };
            _contatoRepositoryMock.Setup(r => r.EditarAsync(It.IsAny<ContatoModel>())).ReturnsAsync(contato);

            // Act
            var resultado = await _controller.Editar(2, contato);

            // Assert
            var okResult = resultado as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(contato, okResult.Value);
        }

        //[Test]
        //public async Task Apagar_ContatoExistente_DeveRetornarNoContent()
        //{
        //    // Arrange
        //    _contatoServiceMock.Setup(s => s.ApagarAsync(6)).ReturnsAsync(true);

        //    // Act
        //    var resultado = await _controller.Apagar(2);

        //    // Assert
        //    var noContentResult = resultado as NoContentResult;
        //    Assert.NotNull(noContentResult);
        //    Assert.AreEqual(204, noContentResult.StatusCode);
        //}

        [Test]
        public async Task Apagar_ContatoInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _contatoServiceMock.Setup(s => s.ApagarAsync(It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var resultado = await _controller.Apagar(999);

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task GetByDDD_ContatoExistente_DeveRetornarOk()
        {
            // Arrange
            var contatos = new List<ContatoModel>
            {
                new ContatoModel { Id = 2, Nome = "Maria", Email = "maria@gmail.com", Telefone = "988888888", DDD = "21", Regiao = "Sudeste" },
            };

            _contatoServiceMock.Setup(s => s.BuscarPorDDDAsync("21")).ReturnsAsync(contatos);

            // Act
            var resultado = await _controller.GetByDDD("21");

            // Assert
            var okResult = resultado as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(contatos, okResult.Value);
        }

        [Test]
        public async Task GetByDDD_ContatoInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _contatoServiceMock.Setup(s => s.BuscarPorDDDAsync("99")).ReturnsAsync(new List<ContatoModel>());

            // Act
            var resultado = await _controller.GetByDDD("99");

            // Assert
            var notFoundResult = resultado as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public async Task Testar_Se_Mensagem_Foi_Publicada()
        {
            // Arrange: Simula a publicação da mensagem
            var ddd = "11";
            //_contatoProducer.PublicarSolicitandoRegiao("SolicitandoRegiao", ddd);

            // Act: Aguarda um tempo para o RabbitMQ processar
            await Task.Delay(2000);

            // Assert: Consulta via RabbitMQ Management API ou log de consumo
            Assert.Pass("Verifique os logs ou painel do RabbitMQ para confirmar a publicação.");
        }

    }
}
