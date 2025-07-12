using RodriguezCalvaRualesMAUIUniWay.Models;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public interface IUsuarioService
    {
        Task<ApiResponse<Usuario>> LoginAsync(LoginRequest request);
        Task<ApiResponse<Usuario>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<List<Usuario>>> GetUsuariosAsync();
        Task<ApiResponse<Usuario>> GetUsuarioByIdAsync(int id);
        Task<ApiResponse<Usuario>> UpdateUsuarioAsync(int id, Usuario usuario);
        Task<ApiResponse<bool>> DeleteUsuarioAsync(int id);
    }
}