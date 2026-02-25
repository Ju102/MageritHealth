using MageritHealth.Data;
using MageritHealth.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MageritHealth.Repositories
{
    public class UsersRepository
    {
        private readonly MageritHealthDbContext context;

        public UsersRepository(MageritHealthDbContext context)
        {
            this.context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email, string password)
        {
            User user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Pass == password);

            return user;
        }
    }
}
