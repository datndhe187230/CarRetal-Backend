using CarRental_BE.Models.NewEntities;
using CarRental_BE.Models.VO.AdminManagement;

namespace CarRental_BE.Models.Mapper
{
    public class AccountMapper
    {
        public static AccountVO ToAccountVO(Account account)
        {
            return new AccountVO
            {
                Id = account.AccountId,
                Email = account.Email,
                Password = account.PasswordHash,
                IsActive = account.IsActive,
                IsEmailVerified = account.IsEmailVerified,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,
                RoleId = account.RoleId
            };
        }
    }
}
