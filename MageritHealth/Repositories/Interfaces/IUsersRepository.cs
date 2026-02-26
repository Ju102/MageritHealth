using MageritHealth.Models;

namespace MageritHealth.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<User> GetUserByEmailAsync(string email, string password);

        Task InsertUser();

        Task DisableUser(int userId);
    }
}
