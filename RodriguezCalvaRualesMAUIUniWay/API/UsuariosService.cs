using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5113/";


        public UsuarioService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };

        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Usuario>>("api/usuarios");
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"api/Usuarios/{id}");
        }


        public async Task<Usuario> CreateUsuarioAsync(Usuario nuevoUsuario)
        {
            var response = await _httpClient.PostAsJsonAsync("api/usuarios", nuevoUsuario);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }

            throw new HttpRequestException($"Error creating user: {response.StatusCode}");
        }

        public async Task UpdateUsuarioAsync(int id, Usuario usuario)
        {
            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/usuarios/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API error: {response.StatusCode} - {error}");
            }
        }


        public async Task DeleteUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error deleting user: {response.StatusCode}");
        }

    }
}
