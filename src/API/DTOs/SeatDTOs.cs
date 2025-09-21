using System;

namespace EventTicketing.API.DTOs
{
    /// <summary>
    /// DTO para mostrar información de un asiento
    /// </summary>
    public class SeatDto
    {
        public Guid Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public string Section { get; set; }
        public decimal PriceMultiplier { get; set; }
    }
}