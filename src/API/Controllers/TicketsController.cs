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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService;

        public TicketsController(ITicketService ticketService, IEventService eventService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var tickets = await _ticketService.GetTicketsByUserAsync(userId);

                var ticketDtos = new List<TicketDto>();
                foreach (var ticket in tickets)
                {
                    var eventEntity = await _eventService.GetEventByIdAsync(ticket.EventId);
                    
                    ticketDtos.Add(new TicketDto
                    {
                        Id = ticket.Id,
                        EventId = ticket.EventId,
                        EventName = eventEntity.Name,
                        EventDate = eventEntity.Date,
                        EventLocation = eventEntity.Location,
                        PurchaseDate = ticket.PurchaseDate,
                        Price = ticket.Price,
                        Code = ticket.Code,
                        IsUsed = ticket.IsUsed,
                        SeatInfo = ticket.SeatId.HasValue ? new SeatInfoDto
                        {
                            Row = ticket.SeatRow,
                            Number = ticket.SeatNumber,
                            Section = ticket.SeatSection
                        } : null
                    });
                }

                return Ok(ticketDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var ticket = await _ticketService.GetTicketByIdAsync(id);

                // Verificar que el ticket pertenece al usuario o el usuario es un administrador
                if (ticket.UserId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                var eventEntity = await _eventService.GetEventByIdAsync(ticket.EventId);

                var ticketDto = new TicketDetailDto
                {
                    Id = ticket.Id,
                    EventId = ticket.EventId,
                    EventName = eventEntity.Name,
                    EventDate = eventEntity.Date,
                    EventLocation = eventEntity.Location,
                    PurchaseDate = ticket.PurchaseDate,
                    Price = ticket.Price,
                    Code = ticket.Code,
                    IsUsed = ticket.IsUsed,
                    SeatInfo = ticket.SeatId.HasValue ? new SeatInfoDto
                    {
                        Row = ticket.SeatRow,
                        Number = ticket.SeatNumber,
                        Section = ticket.SeatSection
                    } : null,
                    QrCodeUrl = $"/api/tickets/{ticket.Id}/qrcode"
                };

                return Ok(ticketDto);
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PurchaseTicket([FromBody] PurchaseTicketDto purchaseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var ticket = new Ticket(purchaseDto.EventId, userId);

                if (purchaseDto.SeatId.HasValue)
                {
                    await _ticketService.PurchaseTicketWithSeatAsync(ticket, purchaseDto.SeatId.Value);
                }
                else
                {
                    await _ticketService.PurchaseTicketAsync(ticket);
                }

                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, new { Id = ticket.Id });
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelTicket(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var ticket = await _ticketService.GetTicketByIdAsync(id);

                // Verificar que el ticket pertenece al usuario o el usuario es un administrador
                if (ticket.UserId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                await _ticketService.CancelTicketAsync(id);

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

        [HttpGet("verify/{code}")]
        [Authorize(Roles = "Organizer,Administrator")]
        public async Task<IActionResult> VerifyTicket(string code)
        {
            try
            {
                var ticket = await _ticketService.VerifyTicketAsync(code);
                var eventEntity = await _eventService.GetEventByIdAsync(ticket.EventId);

                var verificationResult = new TicketVerificationDto
                {
                    IsValid = true,
                    IsUsed = ticket.IsUsed,
                    EventName = eventEntity.Name,
                    EventDate = eventEntity.Date,
                    EventLocation = eventEntity.Location,
                    SeatInfo = ticket.SeatId.HasValue ? new SeatInfoDto
                    {
                        Row = ticket.SeatRow,
                        Number = ticket.SeatNumber,
                        Section = ticket.SeatSection
                    } : null
                };

                return Ok(verificationResult);
            }
            catch (KeyNotFoundException)
            {
                return Ok(new TicketVerificationDto { IsValid = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpPost("mark-used/{code}")]
        [Authorize(Roles = "Organizer,Administrator")]
        public async Task<IActionResult> MarkTicketAsUsed(string code)
        {
            try
            {
                await _ticketService.MarkTicketAsUsedAsync(code);
                return Ok(new { Success = true });
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

        [HttpGet("{id}/qrcode")]
        [Authorize]
        public async Task<IActionResult> GetTicketQrCode(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var ticket = await _ticketService.GetTicketByIdAsync(id);

                // Verificar que el ticket pertenece al usuario o el usuario es un administrador u organizador
                if (ticket.UserId != userId && !User.IsInRole("Administrator") && !User.IsInRole("Organizer"))
                    return Forbid();

                var qrCodeImage = await _ticketService.GenerateTicketQrCodeAsync(id);
                return File(qrCodeImage, "image/png");
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

        [HttpGet("{id}/pdf")]
        [Authorize]
        public async Task<IActionResult> GetTicketPdf(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                var ticket = await _ticketService.GetTicketByIdAsync(id);

                // Verificar que el ticket pertenece al usuario o el usuario es un administrador
                if (ticket.UserId != userId && !User.IsInRole("Administrator"))
                    return Forbid();

                var pdfDocument = await _ticketService.GenerateTicketPdfAsync(id);
                return File(pdfDocument, "application/pdf", $"ticket-{id}.pdf");
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
    }
}