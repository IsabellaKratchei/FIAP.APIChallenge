using FIAP.APIContato.Models;
using FIAP.APIContato.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.APIContato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContatoController : ControllerBase
    {
        private readonly IContatoService _contatoService;
        private readonly IContatoRepository _contatoRepository;

        public ContatoController(IContatoService contatoService, IContatoRepository contatoRepository)
        {
            _contatoService = contatoService;
            _contatoRepository = contatoRepository;
        }

        // M�todo para buscar todos os contatos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var contatos = await _contatoService.BuscarTodosAsync();
                return Ok(contatos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar contatos: {ex.Message}");
            }
        }

        // M�todo para buscar um contato por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var contato = await _contatoService.BuscarPorIdAsync(id);
                if (contato == null)
                {
                    return NotFound($"Contato com ID {id} n�o encontrado.");
                }
                return Ok(contato);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar contato: {ex.Message}");
            }
        }

        // M�todo para adicionar um novo contato
        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(ContatoModel contato)
        {
            if (contato == null)
            {
                return BadRequest("Contato n�o pode ser nulo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var novoContato = await _contatoRepository.AdicionarAsync(contato);
                return CreatedAtAction(nameof(GetById), new { id = novoContato.Id }, novoContato);
            }
            catch (InvalidOperationException ex)
            {
                // Exce��o espec�fica
                return BadRequest($"Erro de opera��o inv�lida: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Exce��o gen�rica
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro interno ao adicionar contato: {ex.Message}");
            }
        }

        // M�todo para editar um contato existente
        [HttpPut("Editar/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ContatoModel contato)
        {
            try
            {
                if (contato == null)
                {
                    return BadRequest("Contato n�o pode ser nulo.");
                }

                contato.Id = id; // Garantir que o ID seja o mesmo
                var contatoEditado = await _contatoService.EditarAsync(contato);

                return Ok(contatoEditado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao editar contato: {ex.Message}");
            }
        }

        // M�todo para excluir um contato
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool sucesso = await _contatoService.ApagarAsync(id);
                if (!sucesso)
                {
                    return NotFound($"Contato com ID {id} n�o encontrado.");
                }

                return NoContent(); // Retorna 204 No Content em caso de exclus�o bem-sucedida
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir contato: {ex.Message}");
            }
        }

        // M�todo para buscar contatos por DDD
        [HttpGet("ddd/{ddd}")]
        public async Task<IActionResult> GetByDDD(string ddd)
        {
            try
            {
                var contatos = await _contatoService.BuscarPorDDDAsync(ddd);
                if (contatos == null || contatos.Count() == 0)
                {
                    return NotFound($"Nenhum contato encontrado para o DDD {ddd}.");
                }
                return Ok(contatos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar contatos por DDD: {ex.Message}");
            }
        }
    }
}