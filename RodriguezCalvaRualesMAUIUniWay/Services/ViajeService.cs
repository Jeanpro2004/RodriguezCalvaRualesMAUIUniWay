using RodriguezCalvaRualesMAUIUniWay.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class ViajeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _viajesFile;
        private const string ApiBaseUrl = "http://localhost:5113/";

        public ViajeService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };

            var appDataPath = FileSystem.AppDataDirectory;
            _viajesFile = Path.Combine(appDataPath, "viajes_locales.json");
        }

        #region API Methods

        public async Task<List<ViajeModel>> GetViajesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ViajeModel>>("api/viajes") ?? new List<ViajeModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API GetViajes: {ex.Message}");
                return await GetViajesLocalesAsync();
            }
        }

        public async Task<ViajeModel> GetViajeByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ViajeModel>($"api/viajes/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API GetViajeById: {ex.Message}");
                var viajesLocales = await GetViajesLocalesAsync();
                return viajesLocales.FirstOrDefault(v => v.Id == id);
            }
        }

        public async Task<ViajeModel> CreateViajeAsync(ViajeModel viaje)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/viajes", viaje);
                if (response.IsSuccessStatusCode)
                {
                    var viajeCreado = await response.Content.ReadFromJsonAsync<ViajeModel>();
                    await GuardarViajeLocalAsync(viajeCreado);
                    return viajeCreado;
                }
                throw new HttpRequestException($"Error creating viaje: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API CreateViaje: {ex.Message}");
                // Guardar solo localmente si falla la API
                viaje.Id = new Random().Next(1000, 9999);
                await GuardarViajeLocalAsync(viaje);
                return viaje;
            }
        }

        public async Task<bool> UpdateViajeAsync(int id, ViajeModel viaje)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/viajes/{id}", viaje);
                if (response.IsSuccessStatusCode)
                {
                    await GuardarViajeLocalAsync(viaje);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API UpdateViaje: {ex.Message}");
                await GuardarViajeLocalAsync(viaje);
                return true; // Asumimos éxito local
            }
        }

        public async Task<bool> DeleteViajeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/viajes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    await EliminarViajeLocalAsync(id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API DeleteViaje: {ex.Message}");
                await EliminarViajeLocalAsync(id);
                return true;
            }
        }

        #endregion

        #region Local Storage Methods

        public async Task<List<ViajeModel>> GetViajesLocalesAsync()
        {
            try
            {
                if (File.Exists(_viajesFile))
                {
                    var json = await File.ReadAllTextAsync(_viajesFile);
                    return JsonSerializer.Deserialize<List<ViajeModel>>(json) ?? new List<ViajeModel>();
                }
                return new List<ViajeModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetViajesLocales: {ex.Message}");
                return new List<ViajeModel>();
            }
        }

        public async Task<bool> GuardarViajeLocalAsync(ViajeModel viaje)
        {
            try
            {
                var viajes = await GetViajesLocalesAsync();

                var existente = viajes.FirstOrDefault(v => v.Id == viaje.Id);
                if (existente != null)
                {
                    var index = viajes.IndexOf(existente);
                    viajes[index] = viaje;
                }
                else
                {
                    viajes.Add(viaje);
                }

                var json = JsonSerializer.Serialize(viajes, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_viajesFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GuardarViajeLocal: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarViajeLocalAsync(int id)
        {
            try
            {
                var viajes = await GetViajesLocalesAsync();
                var viaje = viajes.FirstOrDefault(v => v.Id == id);

                if (viaje != null)
                {
                    viajes.Remove(viaje);
                    var json = JsonSerializer.Serialize(viajes, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(_viajesFile, json);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error EliminarViajeLocal: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Search and Filter Methods

        public async Task<List<ViajeModel>> BuscarViajesAsync(string origen, string destino, DateTime fecha)
        {
            var todosLosViajes = await GetViajesAsync();

            return todosLosViajes.Where(v =>
                (string.IsNullOrEmpty(origen) || v.Origen.Contains(origen, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(destino) || v.Destino.Contains(destino, StringComparison.OrdinalIgnoreCase)) &&
                v.Fecha.Date == fecha.Date &&
                v.EspaciosDisponibles > 0 &&
                v.Estado == "Activo"
            ).ToList();
        }

        public async Task<List<ViajeModel>> GetViajesPorConductorAsync(int conductorId)
        {
            var todosLosViajes = await GetViajesAsync();
            return todosLosViajes.Where(v => v.ConductorId == conductorId).ToList();
        }

        public async Task<List<ViajeModel>> GetViajesPopularesAsync()
        {
            var todosLosViajes = await GetViajesAsync();
            return todosLosViajes
                .Where(v => v.Estado == "Activo" && v.EspaciosDisponibles > 0)
                .OrderByDescending(v => v.EspaciosTotales - v.EspaciosDisponibles)
                .Take(5)
                .ToList();
        }

        #endregion
    }
}