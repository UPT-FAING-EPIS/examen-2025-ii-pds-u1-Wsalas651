using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;
using EventTicketing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User> GetUserWithTicketsAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Tickets)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}