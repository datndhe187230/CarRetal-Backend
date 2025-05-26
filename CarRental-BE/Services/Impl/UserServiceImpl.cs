using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VOs.User;
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

    }
}
