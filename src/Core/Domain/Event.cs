using System;
using System.Collections.Generic;

namespace EventTicketing.Core.Domain
{
    /// <summary>
    /// Representa un evento para el cual se pueden vender entradas
    /// </summary>
    public class Event
        {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public DateTime Date { get; private set; }
        public string? Location { get; private set; }
        public string? Category { get; private set; }
        public int TotalCapacity { get; private set; }
        public decimal BasePrice { get; private set; }
        public bool HasNumberedSeats { get; private set; }
        public Guid OrganizerId { get; private set; }
        public List<Ticket> IssuedTickets { get; private set; } = new List<Ticket>();
        public List<Seat> Seats { get; private set; } = new List<Seat>();

        // Constructor privado para EF Core
        private Event() { }

        // Constructor para crear un nuevo evento
        public Event(string name, string description, DateTime date, string location, 
                   string category, int totalCapacity, decimal basePrice, 
                   bool hasNumberedSeats, Guid organizerId)
        {
            // Validaciones de dominio
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del evento no puede estar vacío", nameof(name));
            
            if (date < DateTime.Now)
                throw new ArgumentException("La fecha del evento no puede ser en el pasado", nameof(date));
            
            if (totalCapacity <= 0)
                throw new ArgumentException("La capacidad debe ser mayor a cero", nameof(totalCapacity));
            
            if (basePrice < 0)
                throw new ArgumentException("El precio base no puede ser negativo", nameof(basePrice));

            // Asignación de propiedades
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Date = date;
            Location = location;
            Category = category;
            TotalCapacity = totalCapacity;
            BasePrice = basePrice;
            HasNumberedSeats = hasNumberedSeats;
            OrganizerId = organizerId;
        }

        // Métodos de dominio
        public void AddSeat(Seat seat)
        {
            if (!HasNumberedSeats)
                throw new InvalidOperationException("No se pueden agregar asientos a un evento sin asientos numerados");
            
            if (Seats.Count >= TotalCapacity)
                throw new InvalidOperationException("No se pueden agregar más asientos que la capacidad total");
            
            Seats.Add(seat);
        }

        public bool IsAvailable()
        {
            return Date > DateTime.Now && IssuedTickets.Count < TotalCapacity;
        }

        public int AvailableSeats()
        {
            return TotalCapacity - IssuedTickets.Count;
        }

        public void UpdateDetails(string name, string description, DateTime date, string location, string category)
        {
            // Solo permitir actualizar si no hay tickets vendidos
            if (IssuedTickets.Count > 0)
                throw new InvalidOperationException("No se puede modificar un evento con entradas vendidas");

            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            if (date > DateTime.Now)
                Date = date;
            else
                throw new ArgumentException("La fecha del evento no puede ser en el pasado", nameof(date));

            if (!string.IsNullOrWhiteSpace(location))
                Location = location;

            if (!string.IsNullOrWhiteSpace(category))
                Category = category;
        }
    }
}