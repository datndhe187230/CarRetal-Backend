namespace CarRental_BE.Exceptions
{
    public class AppException : Exception
    {
        public int ErrorCode { get; }
        public int StatusCode { get; }

        public AppException(string message, int errorCode, int statusCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
