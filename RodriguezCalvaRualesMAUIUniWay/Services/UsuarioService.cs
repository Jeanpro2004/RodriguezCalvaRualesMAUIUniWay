using RodriguezCalvaRualesMAUIUniWay.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogService _logService;
        private const string ApiBaseUrl = "http://localhost:5113/";

        public UsuarioService(HttpClient httpClient, ILogService logService)
        {
            _httpClient = httpClient;
            _logService = logService;
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
        }

        public async Task<ApiResponse<Usuario>> LoginAsync(LoginRequest request)
        {
            try
            {
                await _logService.LogAsync(LogLevel.Info, "LOGIN_ATTEMPT", $"Intento de login para usuario: {request.Correo}", request.Correo);

                // Buscar usuario por email y validar contraseña
                var usuarios = await _httpClient.GetFromJsonAsync<List<Usuario>>("api/usuarios");
                var usuario = usuarios?.FirstOrDefault(u => u.Correo.Equals(request.Correo, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    await _logService.LogAsync(LogLevel.Warning, "LOGIN_FAILED", "Usuario no encontrado", request.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Usuario no encontrado"
                    };
                }

                // En un escenario real, la contraseña debería estar hasheada
                if (usuario.Contrasena != request.Contrasena)
                {
                    await _logService.LogAsync(LogLevel.Warning, "LOGIN_FAILED", "Contraseña incorrecta", request.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Contraseña incorrecta"
                    };
                }

                await _logService.LogAsync(LogLevel.Info, "LOGIN_SUCCESS", "Login exitoso", request.Correo, null, new { UserId = usuario.Id, EsConductor = usuario.EsConductor });

                return new ApiResponse<Usuario>
                {
                    IsSuccess = true,
                    Message = "Login exitoso",
                    Data = usuario
                };
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "LOGIN_ERROR", "Error durante el login", request.Correo, ex.ToString());
                return new ApiResponse<Usuario>
                {
                    IsSuccess = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Usuario>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                await _logService.LogAsync(LogLevel.Info, "REGISTER_ATTEMPT", $"Intento de registro para usuario: {request.Correo}", request.Correo);

                // Validaciones
                if (request.Contrasena != request.ConfirmarContrasena)
                {
                    await _logService.LogAsync(LogLevel.Warning, "REGISTER_FAILED", "Las contraseñas no coinciden", request.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Las contraseñas no coinciden"
                    };
                }

                if (!request.AceptaTerminos)
                {
                    await _logService.LogAsync(LogLevel.Warning, "REGISTER_FAILED", "No acepta términos y condiciones", request.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Debe aceptar los términos y condiciones"
                    };
                }

                // Verificar si el usuario ya existe
                var usuariosExistentes = await _httpClient.GetFromJsonAsync<List<Usuario>>("api/usuarios");
                if (usuariosExistentes?.Any(u => u.Correo.Equals(request.Correo, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    await _logService.LogAsync(LogLevel.Warning, "REGISTER_FAILED", "El email ya está registrado", request.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "El email ya está registrado"
                    };
                }

                var nuevoUsuario = new Usuario
                {
                    IdBanner = request.IdBanner,
                    Nombre = request.Nombre,
                    Correo = request.Correo,
                    Telefono = request.Telefono,
                    Contrasena = request.Contrasena, // En producción, hashear la contraseña
                    EsConductor = request.EsConductor
                };

                var response = await _httpClient.PostAsJsonAsync("api/usuarios", nuevoUsuario);

                if (response.IsSuccessStatusCode)
                {
                    var usuarioCreado = await response.Content.ReadFromJsonAsync<Usuario>();
                    await _logService.LogAsync(LogLevel.Info, "REGISTER_SUCCESS", "Registro exitoso", request.Correo, null, new { UserId = usuarioCreado?.Id, EsConductor = request.EsConductor });

                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = true,
                        Message = "Usuario registrado exitosamente",
                        Data = usuarioCreado
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await _logService.LogAsync(LogLevel.Error, "REGISTER_FAILED", "Error en la API al crear usuario", request.Correo, errorContent);

                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Error al registrar usuario",
                        ErrorDetails = errorContent
                    };
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "REGISTER_ERROR", "Error durante el registro", request.Correo, ex.ToString());
                return new ApiResponse<Usuario>
                {
                    IsSuccess = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<Usuario>>> GetUsuariosAsync()
        {
            try
            {
                var usuarios = await _httpClient.GetFromJsonAsync<List<Usuario>>("api/usuarios");
                return new ApiResponse<List<Usuario>>
                {
                    IsSuccess = true,
                    Data = usuarios ?? new List<Usuario>()
                };
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "GET_USUARIOS_ERROR", "Error al obtener usuarios", null, ex.ToString());
                return new ApiResponse<List<Usuario>>
                {
                    IsSuccess = false,
                    Message = "Error al obtener usuarios",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Usuario>> GetUsuarioByIdAsync(int id)
        {
            try
            {
                var usuario = await _httpClient.GetFromJsonAsync<Usuario>($"api/usuarios/{id}");
                return new ApiResponse<Usuario>
                {
                    IsSuccess = usuario != null,
                    Data = usuario,
                    Message = usuario != null ? "Usuario encontrado" : "Usuario no encontrado"
                };
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "GET_USUARIO_ERROR", $"Error al obtener usuario ID: {id}", null, ex.ToString());
                return new ApiResponse<Usuario>
                {
                    IsSuccess = false,
                    Message = "Error al obtener usuario",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ApiResponse<Usuario>> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/usuarios/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    await _logService.LogAsync(LogLevel.Info, "UPDATE_USER_SUCCESS", $"Usuario actualizado ID: {id}", usuario.Correo);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = true,
                        Message = "Usuario actualizado exitosamente",
                        Data = usuario
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await _logService.LogAsync(LogLevel.Error, "UPDATE_USER_FAILED", $"Error al actualizar usuario ID: {id}", usuario.Correo, errorContent);
                    return new ApiResponse<Usuario>
                    {
                        IsSuccess = false,
                        Message = "Error al actualizar usuario",
                        ErrorDetails = errorContent
                    };
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "UPDATE_USER_ERROR", $"Error al actualizar usuario ID: {id}", usuario.Correo, ex.ToString());
                return new ApiResponse<Usuario>
                {
                    IsSuccess = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteUsuarioAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/usuarios/{id}");

                if (response.IsSuccessStatusCode)
                {
                    await _logService.LogAsync(LogLevel.Info, "DELETE_USER_SUCCESS", $"Usuario eliminado ID: {id}");
                    return new ApiResponse<bool>
                    {
                        IsSuccess = true,
                        Message = "Usuario eliminado exitosamente",
                        Data = true
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await _logService.LogAsync(LogLevel.Error, "DELETE_USER_FAILED", $"Error al eliminar usuario ID: {id}", null, errorContent);
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Error al eliminar usuario",
                        ErrorDetails = errorContent
                    };
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, "DELETE_USER_ERROR", $"Error al eliminar usuario ID: {id}", null, ex.ToString());
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                };
            }
        }
    }
}