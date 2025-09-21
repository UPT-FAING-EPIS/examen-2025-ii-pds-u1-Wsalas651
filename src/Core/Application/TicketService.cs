using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;

namespace EventTicketing.Core.Application
{
    /// <summary>
    /// Implementación del servicio de tickets
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            IEventRepository eventRepository,
            IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Ticket> PurchaseTicketAsync(Guid eventId, Guid userId, Guid? seatId = null)
        {
            // Verificar que el evento existe y está disponible
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            if (!eventEntity.IsAvailable())
                throw new InvalidOperationException("El evento no está disponible para compra de entradas");

            // Verificar que el usuario existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

            // Si se especifica un asiento, verificar que está disponible
            if (seatId.HasValue)
            {
                if (!eventEntity.HasNumberedSeats)
                    throw new InvalidOperationException("El evento no tiene asientos numerados");

                var isSeatReserved = await _ticketRepository.IsSeatReservedAsync(eventId, seatId.Value);
                if (isSeatReserved)
                    throw new InvalidOperationException("El asiento seleccionado ya está reservado");
            }
            else if (eventEntity.HasNumberedSeats)
            {
                throw new InvalidOperationException("Debe seleccionar un asiento para este evento");
            }

            // Calcular precio
            decimal price = eventEntity.BasePrice;
            if (seatId.HasValue)
            {
                // Buscar el asiento para aplicar el multiplicador de precio
                var seat = eventEntity.Seats.Find(s => s.Id == seatId.Value);
                if (seat != null)
                {
                    price *= seat.PriceMultiplier;
                    seat.Reserve(); // Marcar el asiento como reservado
                }
            }

            // Crear el ticket
            var ticket = new Ticket(eventId, userId, price, seatId);
            await _ticketRepository.AddAsync(ticket);

            return ticket;
        }

        public async Task<Ticket> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket con ID {ticketId} no encontrado");

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId)
        {
            return await _ticketRepository.GetTicketsByUserAsync(userId);
        }

        public async Task<bool> VerifyTicketAsync(string ticketCode)
        {
            var ticket = await _ticketRepository.GetTicketByCodeAsync(ticketCode);
            if (ticket == null)
                return false;

            // Verificar que el ticket no ha sido usado y que el evento no ha pasado
            var eventEntity = await _eventRepository.GetByIdAsync(ticket.EventId);
            if (eventEntity == null)
                return false;

            return ticket.IsValid(eventEntity.Date);
        }

        public async Task MarkTicketAsUsedAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket con ID {ticketId} no encontrado");

            ticket.MarkAsUsed();
            await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task<byte[]> GenerateTicketPdfAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket con ID {ticketId} no encontrado");

            var eventEntity = await _eventRepository.GetByIdAsync(ticket.EventId);
            var user = await _userRepository.GetByIdAsync(ticket.UserId);

            // Aquí se generaría el PDF con la información del ticket
            // Esta es una implementación simulada
            return new byte[0]; // En una implementación real, se devolvería el PDF generado
        }

        public async Task<byte[]> GenerateTicketQrCodeAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket con ID {ticketId} no encontrado");

            // Aquí se generaría el código QR con la información del ticket
            // Esta es una implementación simulada
            return new byte[0]; // En una implementación real, se devolvería la imagen del QR generado
        }

        public async Task CancelTicketAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket con ID {ticketId} no encontrado");

            var eventEntity = await _eventRepository.GetByIdAsync(ticket.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {ticket.EventId} no encontrado");

            // Verificar que el evento no ha pasado
            if (eventEntity.Date <= DateTime.Now)
                throw new InvalidOperationException("No se puede cancelar un ticket para un evento que ya ha pasado");

            // Si el ticket tiene un asiento asignado, liberarlo
            if (ticket.SeatId.HasValue)
            {
                var seat = eventEntity.Seats.Find(s => s.Id == ticket.SeatId.Value);
                if (seat != null)
                    seat.Release();
            }

            // Eliminar el ticket
            await _ticketRepository.DeleteAsync(ticket);

            // En una implementación real, aquí se procesaría el reembolso al usuario
        }
    }
}