using FIAP.APIRegiao.Events;
using FIAP.APIRegiao.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.APIRegiao.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegiaoController : ControllerBase
    {
        private readonly IRegiaoService _regiaoService;
        //private readonly RegiaoProducer _regiaoProducer;

        public RegiaoController(IRegiaoService regiaoService
            //, RegiaoProducer regiaoProducer
            )
        {
            _regiaoService = regiaoService;
            //_regiaoProducer = regiaoProducer;
        }

        [HttpGet("{ddd}")]
        public async Task<IActionResult> ObterRegiaoPorDDD(string ddd)
        {
            var regiao = await _regiaoService.ObterRegiaoPorDDDAsync(ddd);

            if (regiao == null)
                return NotFound(new { Message = $"DDD '{ddd}' não encontrado." });

            // Publicar mensagem
            //_regiaoProducer.PublicarMensagem($"Consulta realizada para DDD: {ddd}, Região: {regiao.Regiao}");

            return Ok(regiao);
        }
    }
}
