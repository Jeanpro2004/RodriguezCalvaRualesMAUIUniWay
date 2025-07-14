using RodriguezCalvaRualesMAUIUniWay.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class VehiculoService
    {
        private readonly string _vehiculosFile;
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5113/";

        public VehiculoService()
        {
            var appDataPath = FileSystem.AppDataDirectory;
            _vehiculosFile = Path.Combine(appDataPath, "vehiculos_locales.json");

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
        }

        #region Métodos Locales (Archivos)

        public async Task<List<VehiculoModel>> GetVehiculosLocalesAsync()
        {
            try
            {
                if (File.Exists(_vehiculosFile))
                {
                    var json = await File.ReadAllTextAsync(_vehiculosFile);
                    return JsonSerializer.Deserialize<List<VehiculoModel>>(json) ?? new List<VehiculoModel>();
                }
                return new List<VehiculoModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo vehículos locales: {ex.Message}");
                return new List<VehiculoModel>();
            }
        }

        public async Task<bool> GuardarVehiculoLocalAsync(VehiculoModel vehiculo)
        {
            try
            {
                var vehiculos = await GetVehiculosLocalesAsync();

                var existente = vehiculos.FirstOrDefault(v => v.Id == vehiculo.Id);
                if (existente != null)
                {
                    var index = vehiculos.IndexOf(existente);
                    vehiculos[index] = vehiculo;
                }
                else
                {
                    vehiculo.Id = vehiculos.Count > 0 ? vehiculos.Max(v => v.Id) + 1 : 1;
                    vehiculos.Add(vehiculo);
                }

                var json = JsonSerializer.Serialize(vehiculos, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_vehiculosFile, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando vehículo local: {ex.Message}");
                return false;
            }
        }

        public async Task<List<VehiculoModel>> GetVehiculosPorConductorAsync(int conductorId)
        {
            var vehiculos = await GetVehiculosLocalesAsync();
            return vehiculos.Where(v => v.ConductorId == conductorId).ToList();
        }

        public async Task<bool> EliminarVehiculoLocalAsync(int vehiculoId)
        {
            try
            {
                var vehiculos = await GetVehiculosLocalesAsync();
                var vehiculo = vehiculos.FirstOrDefault(v => v.Id == vehiculoId);

                if (vehiculo != null)
                {
                    vehiculos.Remove(vehiculo);
                    var json = JsonSerializer.Serialize(vehiculos, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(_vehiculosFile, json);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando vehículo: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Métodos API (Opcionales)

        public async Task<VehiculoModel> CreateVehiculoApiAsync(VehiculoModel vehiculo)
        {
            try
            {
                // Mapear a DTO de la API si es necesario
                var vehiculoDto = new
                {
                    Marca = vehiculo.Marca,
                    Modelo = vehiculo.Modelo,
                    Color = vehiculo.Color,
                    Placa = vehiculo.Placa,
                    ConductorId = vehiculo.ConductorId
                };

                var response = await _httpClient.PostAsJsonAsync("api/vehiculos", vehiculoDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<VehiculoModel>();
                    return result;
                }

                throw new HttpRequestException($"Error creating vehicle: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error API CreateVehiculo: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
