using System;

namespace EventTicketing.Core.Domain
{
    /// <summary>
    /// Representa una entrada para un evento
    /// </summary>
    public class Ticket
    {
        public Guid Id { get; private set; }
        public Guid EventId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal Price { get; private set; }
        public string Code { get; private set; }
        public bool IsUsed { get; private set; }
        public Guid? SeatId { get; private set; }
        public string SeatRow { get; private set; }
        public int SeatNumber { get; private set; }
        public string SeatSection { get; private set; }

        // Constructor privado para EF Core
        private Ticket() { }

        // Constructor para crear un nuevo ticket
        public Ticket(Guid eventId, Guid userId, decimal price, Guid? seatId = null)
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("El ID del evento no puede estar vacío", nameof(eventId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("El ID del usuario no puede estar vacío", nameof(userId));
            
            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));

            Id = Guid.NewGuid();
            EventId = eventId;
            UserId = userId;
            PurchaseDate = DateTime.Now;
            Price = price;
            Code = GenerateTicketCode();
            IsUsed = false;
            SeatId = seatId;
        }
        
        // Constructor para crear un ticket con información de asiento
        public Ticket(Guid eventId, Guid userId, decimal price, Guid seatId, string row, int number, string section)
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("El ID del evento no puede estar vacío", nameof(eventId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("El ID del usuario no puede estar vacío", nameof(userId));
            
            if (price < 0)
                throw new ArgumentException("El precio no puede ser negativo", nameof(price));
                
            if (seatId == Guid.Empty)
                throw new ArgumentException("El ID del asiento no puede estar vacío", nameof(seatId));

            Id = Guid.NewGuid();
            EventId = eventId;
            UserId = userId;
            PurchaseDate = DateTime.Now;
            Price = price;
            Code = GenerateTicketCode();
            IsUsed = false;
            SeatId = seatId;
            SeatRow = row;
            SeatNumber = number;
            SeatSection = section;
        }

        // Métodos de dominio
        private string GenerateTicketCode()
        {
            // Genera un código único para el ticket
            return $"{EventId.ToString().Substring(0, 8)}-{DateTime.Now.Ticks.ToString().Substring(0, 8)}";
        }

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new InvalidOperationException("El ticket ya ha sido utilizado");
            
            IsUsed = true;
        }

        public bool IsValid(DateTime eventDate)
        {
            // Un ticket es válido si no ha sido usado y la fecha del evento no ha pasado
            return !IsUsed && DateTime.Now <= eventDate;
        }
    }
}