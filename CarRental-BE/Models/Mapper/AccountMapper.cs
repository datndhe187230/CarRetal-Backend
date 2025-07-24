using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO.AdminManagement;

namespace CarRental_BE.Models.Mapper
{
    public class AccountMapper
    {
        public static AccountVO ToAccountVO(Account account)
        {
            return new AccountVO
            {
                Id = account.Id,
                Email = account.Email,
                Password = account.Password,
                IsActive = account.IsActive,
                IsEmailVerified = account.IsEmailVerified,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,
                RoleId = account.RoleId
            };
        }
    }
}
