using FIAP.APIContato.Models;
using FIAP.APIContato.Repositories;
//using FIAP.APIRegiao.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAP.APIContato.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContatoController : ControllerBase
    {
        private readonly IContatoService _contatoService;
        private readonly IContatoRepository _contatoRepository;
        //private readonly ContatoProducer _contatoProducer;

        public ContatoController(
            IContatoService contatoService, 
            IContatoRepository contatoRepository//,
            //ContatoProducer contatoProducer
            )
        {
            _contatoService = contatoService;
            _contatoRepository = contatoRepository;
            //_contatoProducer = contatoProducer;
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
        [HttpPost("Criar")]
        public async Task<IActionResult> Criar(ContatoModel contato)
        {
            if (contato == null)
            {
                return BadRequest("Contato não pode ser nulo.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var novoContato = await _contatoRepository.AdicionarAsync(contato);
                //_contatoProducer.PublicarMensagem("ContatoCriado",novoContato);
                return CreatedAtAction(nameof(GetById), new { id = novoContato.Id }, novoContato);
            }
            catch (InvalidOperationException ex)
            {
                // Exceção específica
                return BadRequest($"Erro de operação inválida: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Exceção genérica
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro interno ao adicionar contato: {ex.Message}");
            }
        }

        // Método para editar um contato existente
        [HttpPost("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] ContatoModel contato)
        {
            try
            {
                if (contato == null)
                {
                    return BadRequest("Contato não pode ser nulo.");
                }

                contato.Id = id; // Garantir que o ID seja o mesmo
                var contatoEditado = await _contatoRepository.EditarAsync(contato);
                //_contatoProducer.PublicarMensagem("ContatoEditado", contatoEditado);

                return Ok(contatoEditado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao editar contato: {ex.Message}");
            }
        }

        [HttpDelete("Apagar/{id}")]
        public async Task<IActionResult> Apagar(int id)
        {
            try
            {
                // Recupera o contato para ter os dados completos
                var contatoExistente = await _contatoRepository.BuscarPorIdAsync(id);
                if (contatoExistente == null)
                {
                    return NotFound($"Contato com ID {id} não encontrado.");
                }

                bool sucesso = await _contatoService.ApagarAsync(id);
                if (!sucesso)
                {
                    return NotFound($"Contato com ID {id} não encontrado.");
                }
                //_contatoProducer.PublicarMensagem("ContatoExcluido", contatoExistente);

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
