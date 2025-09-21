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
    /// Implementación del repositorio de eventos
    /// </summary>
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(e => e.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(Guid organizerId)
        {
            return await _dbSet
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAvailableEventsAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .Where(e => e.Date > now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAvailableEventsAsync();

            searchTerm = searchTerm.ToLower();
            return await _dbSet
                .Where(e => e.Name.ToLower().Contains(searchTerm) || 
                           e.Description.ToLower().Contains(searchTerm) ||
                           e.Location.ToLower().Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<Event> GetEventWithSeatsAsync(Guid eventId)
        {
            return await _dbSet
                .Include(e => e.Seats)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<Event> GetEventWithTicketsAsync(Guid eventId)
        {
            return await _dbSet
                .Include(e => e.IssuedTickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }
    }
}