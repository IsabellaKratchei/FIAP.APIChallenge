using FIAP.APIContato.Data;
using FIAP.APIContato.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using FIAP.APIRegiao.Events;

namespace FIAP.APIContato.Repositories
{
    public class ContatoRepository : IContatoRepository
    {
        private readonly ContatosDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ContatoProducer _contatoProducer;

        public ContatoRepository(ContatosDbContext dbContext, IHttpClientFactory httpClientFactory, ContatoProducer contatoProducer)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpClient = httpClientFactory.CreateClient("ApiRegiao");
            _contatoProducer = contatoProducer;
        }

        // Buscar contato por ID
        public async Task<ContatoModel> BuscarPorIdAsync(int id)
        {
            return await _dbContext.Contatos.FirstOrDefaultAsync(x => x.Id == id);
        }

        // Buscar todos os contatos
        public async Task<List<ContatoModel>> BuscarTodosAsync()
        {
            return await _dbContext.Contatos.ToListAsync();
        }

        // Adicionar um novo contato
        public async Task<ContatoModel> AdicionarAsync(ContatoModel contato)
        {
            try
            {
                // Validação básica do e-mail
                if (string.IsNullOrWhiteSpace(contato.Email) || !contato.Email.Contains("@"))
                {
                    throw new ArgumentException("O email informado não é válido!");
                }

                // Obter a região correspondente ao DDD
                contato.Regiao = await ObterRegiaoPorDDDAsync(contato.DDD);
                _contatoProducer.PublicarSolicitandoRegiao("Solicitando regiao", contato.DDD);
                if (string.IsNullOrEmpty(contato.Regiao))
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
                }

                // Adicionar contato no banco
                await _dbContext.Contatos.AddAsync(contato);
                await _dbContext.SaveChangesAsync();

                return contato;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar contato: " + ex);
            }
        }

        // Editar um contato existente
        public async Task<ContatoModel> EditarAsync(ContatoModel contato)
        {
            try
            {
                // Verificar se o contato existe
                var contatoBD = await BuscarPorIdAsync(contato.Id);
                if (contatoBD == null)
                {
                    throw new Exception("Contato não encontrado para atualização!");
                }

                _contatoProducer.PublicarSolicitandoRegiao("Solicitando regiao", contato.DDD);
                // Obter a região correspondente ao DDD
                contato.Regiao = await ObterRegiaoPorDDDAsync(contato.DDD);
                if (string.IsNullOrEmpty(contato.Regiao))
                {
                    throw new Exception($"DDD '{contato.DDD}' não encontrado.");
                }

                // Atualizar os dados do contato
                contatoBD.Nome = contato.Nome;
                contatoBD.Email = contato.Email;
                contatoBD.DDD = contato.DDD;
                contatoBD.Telefone = contato.Telefone;
                contatoBD.Regiao = contato.Regiao;

                _dbContext.Contatos.Update(contatoBD);
                await _dbContext.SaveChangesAsync();

                return contatoBD;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao editar contato: " + ex.Message);
            }
        }

        // Apagar um contato
        public async Task<bool> ApagarAsync(int id)
        {
            var contato = await BuscarPorIdAsync(id);
            if (contato == null)
            {
                throw new Exception("Contato não encontrado para exclusão!");
            }

            try
            {
                _dbContext.Contatos.Remove(contato);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao apagar contato: " + ex.Message);
            }
        }

        // Buscar contatos por DDD
        public async Task<List<ContatoModel>> BuscarPorDDDAsync(string ddd)
        {
            return await _dbContext.Contatos.Where(c => c.DDD == ddd).ToListAsync();
        }

        // Obter região de outro microsserviço (via HTTP)
        private async Task<string> ObterRegiaoPorDDDAsync(string ddd)
        {
            _contatoProducer.PublicarSolicitandoRegiao("Solicitando regiao", ddd);
            var response = await _httpClient.GetAsync($"Regiao/{ddd}");
            if (response.IsSuccessStatusCode)
            {
                var conteudo = await response.Content.ReadFromJsonAsync<RegiaoModel>();
                return conteudo?.Regiao;
            }

            return null;
        }
    }
}