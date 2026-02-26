using System.Threading.Tasks;
using MageritHealth.Data;
using MageritHealth.Models;
using MageritHealth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MageritHealth.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MageritHealthDbContext context;

        public UsersRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email, string password)
        {
            var consulta =
                from data in this.context.Users
                where data.Email == email && data.Pass == password
                select data;
            try
            {
                User user = await consulta.FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task InsertUser() { }

        public async Task DisableUser(int userId) { }
    }
}
