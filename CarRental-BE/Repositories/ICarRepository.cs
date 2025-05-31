using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface ICarRepository
    {

        Task<List<Car>> GetAccountId(Guid accountId);


    }
}
