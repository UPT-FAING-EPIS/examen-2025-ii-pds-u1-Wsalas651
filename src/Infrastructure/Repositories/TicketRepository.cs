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
    /// Implementación del repositorio de tickets
    /// </summary>
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByEventAsync(Guid eventId)
        {
            return await _dbSet
                .Where(t => t.EventId == eventId)
                .ToListAsync();
        }

        public async Task<Ticket> GetTicketByCodeAsync(string ticketCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Code == ticketCode);
        }

        public async Task<bool> IsSeatReservedAsync(Guid eventId, Guid seatId)
        {
            return await _dbSet
                .AnyAsync(t => t.EventId == eventId && t.SeatId == seatId);
        }

        public async Task<int> GetTicketsSoldCountAsync(Guid eventId)
        {
            return await _dbSet
                .CountAsync(t => t.EventId == eventId);
        }
    }
}