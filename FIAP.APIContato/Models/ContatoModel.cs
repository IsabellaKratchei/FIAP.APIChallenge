using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FIAP.APIContato.Models
{
    public class ContatoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Digite o nome do contato")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Digite o email do contato")]
        [EmailAddress(ErrorMessage = "O email informado nao é válido!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite o celular do contato")]
        [Phone]
        public string Telefone { get; set; }
        [Required(ErrorMessage = "Digite o DDD do telefone do contato")]
        [ValidateNever]
        public string Regiao { get; set; }
        [Required(ErrorMessage = "DDD é obrigatório.")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "DDD deve conter 2 dígitos.")]
        public string DDD { get; set; }
    }
}
