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

        public ContatoController(IContatoService contatoService)
        {
            _contatoService = contatoService;
        }

        // Método para buscar todos os contatos
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

        // Método para buscar um contato por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var contato = await _contatoService.BuscarPorIdAsync(id);
                if (contato == null)
                {
                    return NotFound($"Contato com ID {id} não encontrado.");
                }
                return Ok(contato);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar contato: {ex.Message}");
            }
        }

        // Método para adicionar um novo contato
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContatoModel contato)
        {
            try
            {
                if (contato == null)
                {
                    return BadRequest("Contato não pode ser nulo.");
                }

                var novoContato = await _contatoService.AdicionarAsync(contato);

                return CreatedAtAction(nameof(GetById), new { id = novoContato.Id }, novoContato);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao adicionar contato: {ex.Message}");
            }
        }

        // Método para editar um contato existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ContatoModel contato)
        {
            try
            {
                if (contato == null)
                {
                    return BadRequest("Contato não pode ser nulo.");
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

        // Método para excluir um contato
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool sucesso = await _contatoService.ApagarAsync(id);
                if (!sucesso)
                {
                    return NotFound($"Contato com ID {id} não encontrado.");
                }

                return NoContent(); // Retorna 204 No Content em caso de exclusão bem-sucedida
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir contato: {ex.Message}");
            }
        }

        // Método para buscar contatos por DDD
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
