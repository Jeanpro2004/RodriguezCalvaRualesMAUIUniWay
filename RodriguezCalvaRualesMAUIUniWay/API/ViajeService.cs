using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class ViajeService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7062/";

        public ViajeService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<Viaje>> GetViajesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Viaje>>("api/Viajes");
        }

        public async Task<Viaje> GetViajeByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Viaje>($"api/Viajes/{id}");
        }

        public async Task<Viaje> CreateViajeAsync(Viaje nuevoViaje)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Viajes", nuevoViaje);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Viaje>();
        }

        public async Task UpdateViajeAsync(int id, Viaje viaje)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Viajes/{id}", viaje);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteViajeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Viajes/{id}");
            response.EnsureSuccessStatusCode();
        }
    }

}
