using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class UsuarioServiceImpl : IUsuarioService
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceImpl(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<ServiceResult<Usuario>> LoginAsync(LoginRequest request)
        {
            try
            {
                var usuarios = await _usuarioService.GetUsuariosAsync();
                var usuario = usuarios?.FirstOrDefault(u =>
                    u.Correo.Equals(request.Correo, StringComparison.OrdinalIgnoreCase) &&
                    u.Contrasena == request.Contrasena);

                if (usuario != null)
                {
                    return new ServiceResult<Usuario>
                    {
                        IsSuccess = true,
                        Data = usuario,
                        Message = "Login exitoso"
                    };
                }

                return new ServiceResult<Usuario>
                {
                    IsSuccess = false,
                    Message = "Email o contraseña incorrectos"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<Usuario>
                {
                    IsSuccess = false,
                    Message = $"Error en el login: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<Usuario>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nombre = request.Nombre,
                    Correo = request.Correo,
                    Telefono = request.Telefono,
                    IdBanner = request.IdBanner,
                    Contrasena = request.Contrasena,
                    EsConductor = request.EsConductor
                };

                var usuarioCreado = await _usuarioService.CreateUsuarioAsync(usuario);

                return new ServiceResult<Usuario>
                {
                    IsSuccess = true,
                    Data = usuarioCreado,
                    Message = "Usuario registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<Usuario>
                {
                    IsSuccess = false,
                    Message = $"Error en el registro: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<Usuario>> GetUsuarioByIdAsync(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
                return new ServiceResult<Usuario>
                {
                    IsSuccess = usuario != null,
                    Data = usuario,
                    Message = usuario != null ? "Usuario encontrado" : "Usuario no encontrado"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<Usuario>
                {
                    IsSuccess = false,
                    Message = $"Error obteniendo usuario: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<Usuario>> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            try
            {
                await _usuarioService.UpdateUsuarioAsync(id, usuario);
                return new ServiceResult<Usuario>
                {
                    IsSuccess = true,
                    Data = usuario,
                    Message = "Usuario actualizado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<Usuario>
                {
                    IsSuccess = false,
                    Message = $"Error actualizando usuario: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<bool>> DeleteUsuarioAsync(int id)
        {
            try
            {
                await _usuarioService.DeleteUsuarioAsync(id);
                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Usuario eliminado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Error eliminando usuario: {ex.Message}"
                };
            }
        }
    }
}