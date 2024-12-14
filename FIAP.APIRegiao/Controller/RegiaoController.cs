using FIAP.APIRegiao.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.APIRegiao.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegioesController : ControllerBase
    {
        private readonly IRegiaoService _regiaoService;

        public RegioesController(IRegiaoService regiaoService)
        {
            _regiaoService = regiaoService;
        }

        [HttpGet("{ddd}")]
        public async Task<IActionResult> ObterRegiaoPorDDD(string ddd)
        {
            var regiao = await _regiaoService.ObterRegiaoPorDDDAsync(ddd);

            if (regiao == null)
                return NotFound(new { Message = $"DDD '{ddd}' não encontrado." });

            return Ok(regiao);
        }
    }
}
