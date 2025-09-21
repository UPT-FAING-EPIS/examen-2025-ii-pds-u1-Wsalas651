using System;

namespace EventTicketing.Core.Domain
{
    /// <summary>
    /// Representa un asiento numerado en un evento
    /// </summary>
    public class Seat
    {
        public Guid Id { get; private set; }
        public Guid EventId { get; private set; }
        public string Row { get; private set; }
        public int Number { get; private set; }
        public string Section { get; private set; }
        public decimal PriceMultiplier { get; private set; }
        public bool IsReserved { get; private set; }

        // Constructor privado para EF Core
        private Seat() { }

        // Constructor para crear un nuevo asiento
        public Seat(Guid eventId, string row, int number, string section, decimal priceMultiplier = 1.0m)
        {
            if (eventId == Guid.Empty)
                throw new ArgumentException("El ID del evento no puede estar vacío", nameof(eventId));
            
            if (string.IsNullOrWhiteSpace(row))
                throw new ArgumentException("La fila no puede estar vacía", nameof(row));
            
            if (number <= 0)
                throw new ArgumentException("El número de asiento debe ser mayor a cero", nameof(number));
            
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("La sección no puede estar vacía", nameof(section));
            
            if (priceMultiplier <= 0)
                throw new ArgumentException("El multiplicador de precio debe ser mayor a cero", nameof(priceMultiplier));

            Id = Guid.NewGuid();
            EventId = eventId;
            Row = row;
            Number = number;
            Section = section;
            PriceMultiplier = priceMultiplier;
            IsReserved = false;
        }

        // Métodos de dominio
        public void Reserve()
        {
            if (IsReserved)
                throw new InvalidOperationException("El asiento ya está reservado");
            
            IsReserved = true;
        }

        public void Release()
        {
            if (!IsReserved)
                throw new InvalidOperationException("El asiento no está reservado");
            
            IsReserved = false;
        }

        public string GetSeatIdentifier()
        {
            return $"{Section}-{Row}{Number}";
        }
    }
}