using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;

namespace EventTicketing.Core.Application
{
    /// <summary>
    /// Implementación del servicio de eventos
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;

        public EventService(IEventRepository eventRepository, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
                throw new ArgumentNullException(nameof(eventEntity));

            await _eventRepository.AddAsync(eventEntity);
            return eventEntity;
        }

        public async Task<Event> UpdateEventAsync(Guid eventId, string name, string description, DateTime date, string location, string category)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            eventEntity.UpdateDetails(name, description, date, location, category);
            await _eventRepository.UpdateAsync(eventEntity);
            return eventEntity;
        }

        public async Task DeleteEventAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            // Verificar si hay tickets vendidos
            var ticketCount = await _ticketRepository.GetTicketsSoldCountAsync(eventId);
            if (ticketCount > 0)
                throw new InvalidOperationException("No se puede eliminar un evento con entradas vendidas");

            await _eventRepository.DeleteAsync(eventEntity);
        }

        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            return eventEntity;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Event>> GetAvailableEventsAsync()
        {
            return await _eventRepository.GetAvailableEventsAsync();
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAvailableEventsAsync();

            return await _eventRepository.SearchEventsAsync(searchTerm);
        }

        public async Task<IEnumerable<Event>> FilterEventsByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return await GetAvailableEventsAsync();

            return await _eventRepository.GetEventsByCategoryAsync(category);
        }

        public async Task<IEnumerable<Event>> FilterEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _eventRepository.GetEventsByDateRangeAsync(startDate, endDate);
        }

        public async Task AddSeatToEventAsync(Guid eventId, Seat seat)
        {
            var eventEntity = await _eventRepository.GetEventWithSeatsAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            if (!eventEntity.HasNumberedSeats)
                throw new InvalidOperationException("No se pueden agregar asientos a un evento sin asientos numerados");

            eventEntity.AddSeat(seat);
            await _eventRepository.UpdateAsync(eventEntity);
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsForEventAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetEventWithSeatsAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");

            if (!eventEntity.HasNumberedSeats)
                throw new InvalidOperationException("El evento no tiene asientos numerados");

            // Filtrar solo los asientos no reservados
            var availableSeats = new List<Seat>();
            foreach (var seat in eventEntity.Seats)
            {
                if (!seat.IsReserved)
                    availableSeats.Add(seat);
            }

            return availableSeats;
        }
    }
}