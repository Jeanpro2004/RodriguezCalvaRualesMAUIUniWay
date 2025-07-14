using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class VehiculoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5113/";

        public VehiculoService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Vehiculo>>("api/Vehiculos");
        }

        public async Task<Vehiculo> GetVehiculoByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Vehiculo>($"api/Vehiculos/{id}");
        }

        public async Task<Vehiculo> CreateVehiculoAsync(Vehiculo nuevoVehiculo)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Vehiculos", nuevoVehiculo);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehiculo>();
        }

        public async Task UpdateVehiculoAsync(int id, Vehiculo vehiculo)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Vehiculos/{id}", vehiculo);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVehiculoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Vehiculos/{id}");
            response.EnsureSuccessStatusCode();
        }

        public Vehiculo GetVehicleByUserId(int userId)
        {
            var response = _httpClient.GetAsync($"api/Vehiculos/user/{userId}").GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var vehiculo = response.Content.ReadFromJsonAsync<Vehiculo>().GetAwaiter().GetResult();
                return vehiculo;
            }
            return null;
        }

    }

}
