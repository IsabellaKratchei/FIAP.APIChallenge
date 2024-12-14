using FIAP.APIRegiao.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIRegiao.Repository
{
    public interface IRegiaoRepository
    {
        Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd);
    }
}
