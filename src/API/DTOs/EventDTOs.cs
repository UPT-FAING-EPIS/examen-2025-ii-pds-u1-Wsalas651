using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventTicketing.API.DTOs
{
    /// <summary>
    /// DTO para mostrar información básica de un evento
    /// </summary>
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public bool HasNumberedSeats { get; set; }
        public int AvailableSeats { get; set; }
    }

    /// <summary>
    /// DTO para mostrar información detallada de un evento, incluyendo asientos disponibles
    /// </summary>
    public class EventDetailDto : EventDto
    {
        public int TotalCapacity { get; set; }
        public List<SeatDto> AvailableSeatsDetail { get; set; }
    }

    /// <summary>
    /// DTO para crear un nuevo evento
    /// </summary>
    public class CreateEventDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder los 200 caracteres")]
        public string Location { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        [StringLength(50, ErrorMessage = "La categoría no puede exceder los 50 caracteres")]
        public string Category { get; set; }

        [Required(ErrorMessage = "La capacidad total es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La capacidad total debe ser mayor a 0")]
        public int TotalCapacity { get; set; }

        [Required(ErrorMessage = "El precio base es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio base debe ser mayor a 0")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Debe especificar si el evento tiene asientos numerados")]
        public bool HasNumberedSeats { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un evento existente
    /// </summary>
    public class UpdateEventDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida")]
        [StringLength(200, ErrorMessage = "La ubicación no puede exceder los 200 caracteres")]
        public string Location { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        [StringLength(50, ErrorMessage = "La categoría no puede exceder los 50 caracteres")]
        public string Category { get; set; }
    }

    /// <summary>
    /// DTO para agregar un asiento a un evento
    /// </summary>
    public class AddSeatDto
    {
        [Required(ErrorMessage = "La fila es requerida")]
        [StringLength(10, ErrorMessage = "La fila no puede exceder los 10 caracteres")]
        public string Row { get; set; }

        [Required(ErrorMessage = "El número es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El número debe ser mayor a 0")]
        public int Number { get; set; }

        [Required(ErrorMessage = "La sección es requerida")]
        [StringLength(50, ErrorMessage = "La sección no puede exceder los 50 caracteres")]
        public string Section { get; set; }

        [Required(ErrorMessage = "El multiplicador de precio es requerido")]
        [Range(0.1, 10.0, ErrorMessage = "El multiplicador de precio debe estar entre 0.1 y 10.0")]
        public decimal PriceMultiplier { get; set; }
    }
}