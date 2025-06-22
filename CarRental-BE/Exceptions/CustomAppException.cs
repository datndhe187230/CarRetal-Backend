namespace CarRental_BE.Exceptions
{

    //USER EXCEPTION - CODE: 10xx 
    public class UsernameExistException : AppException
    {
        public UsernameExistException()
            : base("Username already exists", 1003, 409) { }
    }

    public class EmailExistException : AppException
    {
        public EmailExistException()
            : base("Email already exists", 1004, 409) { }
    }

    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string email)
            : base($"No user found with email: {email}", 1001, 404) { }
    }

    //BOOKING EXCEPTION - CODE : 11xx
    public class BookingNotFoundException : AppException
    {
        public BookingNotFoundException(string bookingNumber)
            : base($"Booking not found: {bookingNumber}", 1101, 404)
        { }
    }

    public class BookingEditException : AppException
    {
        public BookingEditException(string status)
            : base($"Booking cannot be updated because it has status: {status}", 1100, 400)
        {
        }
    }

    public class InvalidOperationException : AppException
    {
        public InvalidOperationException(string message)
            : base(message, 1002, 400) { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, 1005, 401) { }
    }
}
