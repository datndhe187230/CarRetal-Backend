namespace CarRental_BE.Exceptions
{
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
}
