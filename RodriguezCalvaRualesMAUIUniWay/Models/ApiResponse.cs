
namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string ErrorDetails { get; set; } = string.Empty;
    }
}