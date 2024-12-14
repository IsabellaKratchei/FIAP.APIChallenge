using FIAP.APIRegiao.Models;
using FIAP.APIRegiao.Repository;

namespace FIAP.APIRegiao.Service
{
    public class RegiaoService : IRegiaoService
    {
        private readonly IRegiaoRepository _regiaoRepository;

        public RegiaoService(IRegiaoRepository regiaoRepository)
        {
            _regiaoRepository = regiaoRepository;
        }

        public async Task<RegiaoModel> ObterRegiaoPorDDDAsync(string ddd)
        {
            return await _regiaoRepository.BuscarRegiaoPorDDDAsync(ddd);
        }
    }
}
