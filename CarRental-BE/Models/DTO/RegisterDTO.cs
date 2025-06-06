namespace CarRental_BE.Models.DTO
{
    public class RegisterDTO
    {
        public   string  Email { get; set; }
        public   string Password { get; set; }
        public   string ConfirmPassword { get; set; }
        public   string FullName { get; set; }
        public   string PhoneNumber { get; set; }
         
        public int RoleId { get; set; }  
    }
}