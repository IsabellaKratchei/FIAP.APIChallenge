using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FIAP.APIContato.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace APIContato.Testes
{
    [TestFixture]
    public class ContatoControllerTests
    {
        private HttpClient _client;

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        /// <returns>A Task representing the asynchronous operation.</returns>
        [Test]
        public async Task GetContatos_ShouldReturnOkResponseAsync()
        {
            var response = await _client.GetAsync("/api/contatos");

            // Certifique-se de que a resposta tenha sucesso
            response.EnsureSuccessStatusCode();

            // Verifique se o status da resposta é OK
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task PostContato_ValidContato_ShouldReturnCreatedResponseAsync()
        {
            // Criando o objeto de contato dentro do teste
            var contato = new ContatoModel
            {
                Nome = "John Doe",
                Email = "johndoe@example.com",
                DDD = "11",
                Telefone = "234567890",
                Regiao = "Sudeste"
            };

            var response = await _client.PostAsJsonAsync("/api/contatos", contato);

            // Certifique-se de que a resposta tenha sucesso
            response.EnsureSuccessStatusCode();

            // Verifique se o status da resposta é Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
