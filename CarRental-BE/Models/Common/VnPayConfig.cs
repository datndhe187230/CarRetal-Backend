namespace CarRental_BE.Models.Common
{
    public class VnPayConfig
    {
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string Url { get; set; }
        public string ReturnUrl { get; set; }
    }

}
