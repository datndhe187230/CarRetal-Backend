using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VO.AdminManagement;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Repositories;
using CarRental_BE.Services;

namespace CarRental_BE.Services.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IAccountRepository _accountRepository;

        public UserServiceImpl(IUserRepository repository, IAccountRepository accountRepository)
        {
            _repository = repository;
            _accountRepository = accountRepository;
        }

        public async Task<UserProfileVO?> GetUserProfile(Guid id)
        {
            var profile = await _repository.GetById(id);
            return profile == null ? null : UserProfileMapper.ToVO(profile);
        }
        public async Task<UserProfileVO?> UpdateUserProfile(Guid id, UserUpdateDTO dto)
        {
            var profile = await _repository.UpdateUserProfile(id, dto);
            return profile == null ? null : UserProfileMapper.ToVO(profile);
        }
        public async Task<bool> ChangePassword(Guid id, ChangePasswordDTO dto)
        {
            return await _repository.ChangePassword(id, dto);
        }
        public async Task<bool> Register(RegisterDTO dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return false;
            }
            return await _repository.Register(dto);
        }

        public async Task<Account> UpdateUserStatusAsync(Guid id, UserStatusUpdateRequest request, Guid currentUserId)
        {
            var currentUser = await _accountRepository.GetCurrentUserAsync(currentUserId);
            if (currentUser?.Role.Name != "admin")
            {
                throw new UnauthorizedAccessException("Only admins can edit user status.");
            }

            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                return null;
            }

            account.IsActive = request.IsActive;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAccountAsync(account);
            return account;
        }
    }
}
