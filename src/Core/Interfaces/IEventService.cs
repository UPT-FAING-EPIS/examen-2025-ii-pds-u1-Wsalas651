using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de eventos
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Crea un nuevo evento
        /// </summary>
        Task<Event> CreateEventAsync(Event eventEntity);

        /// <summary>
        /// Actualiza un evento existente
        /// </summary>
        Task<Event> UpdateEventAsync(Guid eventId, string name, string description, DateTime date, string location, string category);

        /// <summary>
        /// Elimina un evento
        /// </summary>
        Task DeleteEventAsync(Guid eventId);

        /// <summary>
        /// Obtiene un evento por su ID
        /// </summary>
        Task<Event> GetEventByIdAsync(Guid eventId);

        /// <summary>
        /// Obtiene todos los eventos
        /// </summary>
        Task<IEnumerable<Event>> GetAllEventsAsync();

        /// <summary>
        /// Obtiene eventos disponibles
        /// </summary>
        Task<IEnumerable<Event>> GetAvailableEventsAsync();

        /// <summary>
        /// Busca eventos por término de búsqueda
        /// </summary>
        Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);

        /// <summary>
        /// Filtra eventos por categoría
        /// </summary>
        Task<IEnumerable<Event>> FilterEventsByCategoryAsync(string category);

        /// <summary>
        /// Filtra eventos por rango de fechas
        /// </summary>
        Task<IEnumerable<Event>> FilterEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Agrega un asiento a un evento
        /// </summary>
        Task AddSeatToEventAsync(Guid eventId, Seat seat);

        /// <summary>
        /// Obtiene los asientos disponibles para un evento
        /// </summary>
        Task<IEnumerable<Seat>> GetAvailableSeatsForEventAsync(Guid eventId);
    }
}