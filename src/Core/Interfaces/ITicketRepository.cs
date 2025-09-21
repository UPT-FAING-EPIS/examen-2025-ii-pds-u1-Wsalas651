using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de tickets
    /// </summary>
    public interface ITicketRepository : IRepository<Ticket>
    {
        /// <summary>
        /// Obtiene tickets por usuario
        /// </summary>
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId);

        /// <summary>
        /// Obtiene tickets por evento
        /// </summary>
        Task<IEnumerable<Ticket>> GetTicketsByEventAsync(Guid eventId);

        /// <summary>
        /// Obtiene un ticket por su código
        /// </summary>
        Task<Ticket> GetTicketByCodeAsync(string ticketCode);

        /// <summary>
        /// Verifica si un asiento está reservado para un evento
        /// </summary>
        Task<bool> IsSeatReservedAsync(Guid eventId, Guid seatId);

        /// <summary>
        /// Obtiene el número de tickets vendidos para un evento
        /// </summary>
        Task<int> GetTicketsSoldCountAsync(Guid eventId);
    }
}