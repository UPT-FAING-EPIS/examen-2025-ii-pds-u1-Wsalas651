using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de tickets
    /// </summary>
    public interface ITicketService
    {
        /// <summary>
        /// Compra un ticket para un evento
        /// </summary>
        Task<Ticket> PurchaseTicketAsync(Guid eventId, Guid userId, Guid? seatId = null);

        /// <summary>
        /// Obtiene un ticket por su ID
        /// </summary>
        Task<Ticket> GetTicketByIdAsync(Guid ticketId);

        /// <summary>
        /// Obtiene tickets por usuario
        /// </summary>
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId);

        /// <summary>
        /// Verifica un ticket
        /// </summary>
        Task<bool> VerifyTicketAsync(string ticketCode);

        /// <summary>
        /// Marca un ticket como usado
        /// </summary>
        Task MarkTicketAsUsedAsync(Guid ticketId);

        /// <summary>
        /// Genera un PDF para un ticket
        /// </summary>
        Task<byte[]> GenerateTicketPdfAsync(Guid ticketId);

        /// <summary>
        /// Genera un código QR para un ticket
        /// </summary>
        Task<byte[]> GenerateTicketQrCodeAsync(Guid ticketId);

        /// <summary>
        /// Cancela un ticket y reembolsa al usuario
        /// </summary>
        Task CancelTicketAsync(Guid ticketId);

        /// <summary>
        /// Obtiene un ticket por su código
        /// </summary>
        Task<Ticket> GetTicketByCodeAsync(string ticketCode);
    }
}