using FIAP.APIContato.Models;
using FIAP.APIContato.Repositories;

namespace FIAP.APIContato.Services
{
    public class RegiaoAPIClient : IRegiaoRepository
    {
        private readonly HttpClient _httpClient;

        public RegiaoAPIClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RegiaoModel> BuscarRegiaoPorDDDAsync(string ddd)
        {
            var response = await _httpClient.GetAsync($"Regiao/{ddd}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RegiaoModel>();
            }

            return null;  // Região não encontrada
        }
    }
}
