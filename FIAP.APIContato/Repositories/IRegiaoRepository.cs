using FIAP.APIContato.Models;

namespace FIAP.APIContato.Repositories
{
  public interface IRegiaoRepository
  {
    Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd);
  }
}
