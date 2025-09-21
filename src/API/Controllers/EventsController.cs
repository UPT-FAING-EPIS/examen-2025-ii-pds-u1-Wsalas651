using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.API.DTOs;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] string category = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] string searchTerm = null)
        {
            try
            {
                IEnumerable<Event> events;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    events = await _eventService.SearchEventsAsync(searchTerm);
                }
                else if (!string.IsNullOrWhiteSpace(category))
                {
                    events = await _eventService.FilterEventsByCategoryAsync(category);
                }
                else if (startDate.HasValue && endDate.HasValue)
                {
                    events = await _eventService.FilterEventsByDateRangeAsync(startDate.Value, endDate.Value);
                }
                else
                {
                    events = await _eventService.GetAvailableEventsAsync();
                }

                var eventDtos = new List<EventDto>();
                foreach (var eventEntity in events)
                {
                    eventDtos.Add(new EventDto
                    {
                        Id = eventEntity.Id,
                        Name = eventEntity.Name,
                        Description = eventEntity.Description,
                        Date = eventEntity.Date,
                        Location = eventEntity.Location,
                        Category = eventEntity.Category,
                        BasePrice = eventEntity.BasePrice,
                        HasNumberedSeats = eventEntity.HasNumberedSeats,
                        AvailableSeats = eventEntity.AvailableSeats()
                    });
                }

                return Ok(eventDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            try
            {
                var eventEntity = await _eventService.GetEventByIdAsync(id);
                var availableSeats = await _eventService.GetAvailableSeatsForEventAsync(id);

                var seatDtos = new List<SeatDto>();
                foreach (var seat in availableSeats)
                {
                    seatDtos.Add(new SeatDto
                    {
                        Id = seat.Id,
                        Row = seat.Row,
                        Number = seat.Number,
                        Section = seat.Section,
                        PriceMultiplier = seat.PriceMultiplier
                    });
                }

                var eventDto = new EventDetailDto
                {
                    Id = eventEntity.Id,
                    Name = eventEntity.Name,
                    Description = eventEntity.Description,
                    Date = eventEntity.Date,
                    Location = eventEntity.Location,
                    Category = eventEntity.Category,
                    BasePrice = eventEntity.BasePrice,
                    HasNumberedSeats = eventEntity.HasNumberedSeats,
                    AvailableSeats = eventEntity.AvailableSeats(),
                    TotalCapacity = eventEntity.TotalCapacity,
                    AvailableSeatsDetail = seatDtos
                };

                return Ok(eventDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Organizer,Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                
                var eventEntity = new Event(
                    createEventDto.Name,
                    createEventDto.Description,
                    createEventDto.Date,
                    createEventDto.Location,
                    createEventDto.Category,
                    createEventDto.TotalCapacity,
                    createEventDto.BasePrice,
                    createEventDto.HasNumberedSeats,
                    userId);

                await _eventService.CreateEventAsync(eventEntity);

                return CreatedAtAction(nameof(GetEvent), new { id = eventEntity.Id }, new { Id = eventEntity.Id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Organizer,Administrator")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var eventEntity = await _eventService.GetEventByIdAsync(id);

                // Verificar que el usuario es el organizador del evento o un administrador
                if (eventEntity.OrganizerId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                await _eventService.UpdateEventAsync(
                    id,
                    updateEventDto.Name,
                    updateEventDto.Description,
                    updateEventDto.Date,
                    updateEventDto.Location,
                    updateEventDto.Category);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Organizer,Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var eventEntity = await _eventService.GetEventByIdAsync(id);

                // Verificar que el usuario es el organizador del evento o un administrador
                if (eventEntity.OrganizerId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                await _eventService.DeleteEventAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Organizer,Administrator")]
        [HttpPost("{id}/seats")]
        public async Task<IActionResult> AddSeat(Guid id, [FromBody] AddSeatDto addSeatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var eventEntity = await _eventService.GetEventByIdAsync(id);

                // Verificar que el usuario es el organizador del evento o un administrador
                if (eventEntity.OrganizerId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                var seat = new Seat(
                    id,
                    addSeatDto.Row,
                    addSeatDto.Number,
                    addSeatDto.Section,
                    addSeatDto.PriceMultiplier);

                await _eventService.AddSeatToEventAsync(id, seat);

                return Ok(new { SeatId = seat.Id });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}