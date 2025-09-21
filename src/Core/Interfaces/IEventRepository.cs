using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de eventos
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Obtiene eventos por categoría
        /// </summary>
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category);

        /// <summary>
        /// Obtiene eventos por fecha
        /// </summary>
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene eventos por organizador
        /// </summary>
        Task<IEnumerable<Event>> GetEventsByOrganizerAsync(Guid organizerId);

        /// <summary>
        /// Obtiene eventos disponibles (futuros y con entradas disponibles)
        /// </summary>
        Task<IEnumerable<Event>> GetAvailableEventsAsync();

        /// <summary>
        /// Busca eventos por término de búsqueda en nombre o descripción
        /// </summary>
        Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);

        /// <summary>
        /// Obtiene un evento con todos sus asientos
        /// </summary>
        Task<Event> GetEventWithSeatsAsync(Guid eventId);

        /// <summary>
        /// Obtiene un evento con todas sus entradas vendidas
        /// </summary>
        Task<Event> GetEventWithTicketsAsync(Guid eventId);
    }
}