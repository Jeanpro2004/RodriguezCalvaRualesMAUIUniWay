using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IUsuarioService
    {
        Task<ServiceResult<Usuario>> LoginAsync(LoginRequest request);
        Task<ServiceResult<Usuario>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<Usuario>> GetUsuarioByIdAsync(int id);
        Task<ServiceResult<Usuario>> UpdateUsuarioAsync(int id, Usuario usuario);
        Task<ServiceResult<bool>> DeleteUsuarioAsync(int id);
    }

    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }

    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public bool RecordarContrasena { get; set; }
    }

    public class RegisterRequest
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string IdBanner { get; set; }
        public string Contrasena { get; set; }
        public string ConfirmarContrasena { get; set; }
        public bool EsConductor { get; set; }
        public bool AceptaTerminos { get; set; }
    }
}
