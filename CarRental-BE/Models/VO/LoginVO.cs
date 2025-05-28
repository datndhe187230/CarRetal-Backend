namespace CarRental_BE.Models.VO
{
    public class LoginVO
    {
        public string Token { get; set; }
        public LoginVO(string token)
        {
            Token = token;
        }
        public LoginVO()
        {
        }
    }
}
