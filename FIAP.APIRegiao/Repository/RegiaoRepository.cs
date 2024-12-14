using FIAP.APIRegiao.Models;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIRegiao.Repository
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private readonly RegiaoDbContext _bdContext;

        public RegiaoRepository(RegiaoDbContext bdContext)
        {
            _bdContext = bdContext;
        }

        public async Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd)
        {
            return await _bdContext.DDDs.FirstOrDefaultAsync(x => x.DDD == ddd);
        }
    }
}
