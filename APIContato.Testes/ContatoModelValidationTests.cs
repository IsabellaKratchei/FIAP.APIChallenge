using FIAP.APIContato.Models;
using System.ComponentModel.DataAnnotations;

namespace APIContato.Testes
{
    public class ContatoModelValidationTests
    {
        private ValidationContext _validationContext;

        [SetUp]
        public void Setup()
        {
            // Setup comum para cada teste
        }

        [Test]
        public void DeveRetornarErro_QuandoNomeAusente()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Email = "teste@teste.com",
                Telefone = "9999-9999",
                DDD = "11",
                Regiao = "Sudeste"
            };

            _validationContext = new ValidationContext(contato);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(contato, _validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.One.Matches<ValidationResult>(
                result => result.ErrorMessage == "Digite o nome do contato"));
        }

        [Test]
        public void DeveRetornarErro_QuandoEmailInvalido()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Teste",
                Email = "email-invalido",
                Telefone = "999999999",
                DDD = "11",
                Regiao = "Sudeste"
            };

            _validationContext = new ValidationContext(contato);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(contato, _validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.One.Matches<ValidationResult>(
                result => result.ErrorMessage == "O email informado nao é válido!"));
        }

        [Test]
        public void DeveRetornarErro_QuandoDDDComMaisDe2Digitos()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "9999-9999",
                DDD = "123", // Inválido
                Regiao = "Sudeste"
            };

            _validationContext = new ValidationContext(contato);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(contato, _validationContext, validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(validationResults, Has.One.Matches<ValidationResult>(
                result => result.ErrorMessage == "DDD deve conter 2 dígitos."));
        }

        [Test]
        public void DeveValidarContatoComDadosCorretos()
        {
            // Arrange
            var contato = new ContatoModel
            {
                Nome = "Teste Válido",
                Email = "teste@teste.com",
                Telefone = "9999-9999",
                DDD = "11",
                Regiao = "Sudeste"
            };

            _validationContext = new ValidationContext(contato);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(contato, _validationContext, validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
            Assert.That(validationResults, Is.Empty);
        }
    }
}