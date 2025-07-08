namespace CarRental_BE.Models.VO.AdminManagement
{
    public class AccountVO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool IsEmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int RoleId { get; set; }
    }
}
