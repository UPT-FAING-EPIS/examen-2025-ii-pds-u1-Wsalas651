using System;

namespace EventTicketing.API.DTOs
{
    /// <summary>
    /// DTO para mostrar información básica de un asiento en un ticket
    /// </summary>
    public class SeatInfoDto
    {
        public string Row { get; set; }
        public int Number { get; set; }
        public string Section { get; set; }
    }

    /// <summary>
    /// DTO para mostrar información básica de un ticket
    /// </summary>
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public SeatInfoDto SeatInfo { get; set; }
    }

    /// <summary>
    /// DTO para mostrar información detallada de un ticket
    /// </summary>
    public class TicketDetailDto : TicketDto
    {
        public string QrCodeUrl { get; set; }
    }

    /// <summary>
    /// DTO para comprar un ticket
    /// </summary>
    public class PurchaseTicketDto
    {
        public Guid EventId { get; set; }
        public Guid? SeatId { get; set; }
    }

    /// <summary>
    /// DTO para verificar un ticket
    /// </summary>
    public class TicketVerificationDto
    {
        public bool IsValid { get; set; }
        public bool IsUsed { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; }
        public SeatInfoDto SeatInfo { get; set; }
    }
}