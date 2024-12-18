using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.APIContato.Models
{
  public class RegiaoModel
  {
    public int Id { get; set; }
    public string DDD { get; set; }
    public string Regiao { get; set; }
  }
}
