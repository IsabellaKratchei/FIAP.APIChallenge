using FIAP.APIRegiao.Models;

namespace FIAP.APIRegiao.Repository
{
    public interface IRegiaoRepository
    {
        Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd);
    }
}
