using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class ReservaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7062/";

        public ReservaService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<Reserva>> GetReservasAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Reserva>>("api/Reservas");
        }

        public async Task<Reserva> GetReservaByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Reserva>($"api/Reservas/{id}");
        }

        public async Task<Reserva> CreateReservaAsync(Reserva nuevaReserva)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Reservas", nuevaReserva);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Reserva>();
        }

        public async Task UpdateReservaAsync(int id, Reserva reserva)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Reservas/{id}", reserva);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReservaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Reservas/{id}");
            response.EnsureSuccessStatusCode();
        }
    }

}
