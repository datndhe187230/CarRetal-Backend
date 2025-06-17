
namespace CarRental_BE.Data
{
    public class ApiResponse<T>
    {
        private Exception data;

        public int Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(int status, string message, T? data = default)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
