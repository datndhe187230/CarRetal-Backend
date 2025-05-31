using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VO.User;
using CarRental_BE.Repositories;
using CarRental_BE.Services;

namespace CarRental_BE.Services.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _repository;
  
        public UserServiceImpl(IUserRepository repository)
        {
            _repository = repository;
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
    }
}
