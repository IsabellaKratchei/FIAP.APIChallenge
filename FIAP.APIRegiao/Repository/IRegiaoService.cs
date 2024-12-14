using FIAP.APIRegiao.Models;

namespace FIAP.APIRegiao.Repository
{
    public interface IRegiaoService
    {
        Task<RegiaoModel> ObterRegiaoPorDDDAsync(string ddd);
    }
}
