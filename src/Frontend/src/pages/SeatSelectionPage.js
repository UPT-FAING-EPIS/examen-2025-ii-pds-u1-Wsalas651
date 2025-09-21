import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { eventService } from '../services/eventService';
import { ticketService } from '../services/ticketService';
import './SeatSelectionPage.css';

const SeatSelectionPage = () => {
  const { eventId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  
  // Obtener la cantidad de entradas de la navegación
  const quantity = location.state?.quantity || 1;
  
  const [event, setEvent] = useState(null);
  const [seats, setSeats] = useState([]);
  const [selectedSeats, setSelectedSeats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [timeLeft, setTimeLeft] = useState(600); // 10 minutos en segundos

  // Liberar asientos reservados - mover ANTES de los useEffect y usar useCallback
  const releaseSeats = useCallback(async () => {
    if (selectedSeats.length > 0) {
      try {
        const seatIds = selectedSeats.map(seat => seat.id);
        await ticketService.releaseSeats(eventId, seatIds);
      } catch (err) {
        console.error('Error releasing seats:', err);
      }
    }
  }, [selectedSeats, eventId]);

  // Cargar evento y asientos disponibles
  useEffect(() => {
    const fetchEventAndSeats = async () => {
      try {
        setLoading(true);
        
        // Cargar detalles del evento
        const eventData = await eventService.getEventById(eventId);
        setEvent(eventData);
        
        // Cargar asientos disponibles
        const seatsData = await eventService.getEventSeats(eventId);
        setSeats(seatsData);
      } catch (err) {
        setError('Error al cargar la información del evento. Por favor, intente nuevamente.');
        console.error('Error fetching event and seats:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchEventAndSeats();

    // Limpiar reservas al salir
    return () => {
      if (selectedSeats.length > 0) {
        releaseSeats();
      }
    };
  }, [eventId, releaseSeats, selectedSeats.length]);

  // Temporizador para la reserva
  useEffect(() => {
    if (selectedSeats.length > 0 && timeLeft > 0) {
      const timer = setTimeout(() => {
        setTimeLeft(prevTime => prevTime - 1);
      }, 1000);
      
      return () => clearTimeout(timer);
    } else if (timeLeft === 0 && selectedSeats.length > 0) {
      // Tiempo expirado, liberar asientos
      releaseSeats();
      setSelectedSeats([]);
      setError('El tiempo de reserva ha expirado. Por favor, seleccione asientos nuevamente.');
    }
  }, [timeLeft, selectedSeats, releaseSeats]); // Agregar releaseSeats y selectedSeats.length

  // Formatear tiempo restante
  const formatTimeLeft = () => {
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;
    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
  };

  // Formatear precio
  const formatPrice = (price) => {
    if (price === undefined || price === null) return '';
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2
    }).format(price);
  };

  // Manejar selección de asiento
  const handleSeatClick = async (seat) => {
    if (seat.status === 'occupied') {
      return; // No se puede seleccionar un asiento ocupado
    }
    
    // Si ya está seleccionado, deseleccionarlo
    if (selectedSeats.some(s => s.id === seat.id)) {
      const updatedSeats = selectedSeats.filter(s => s.id !== seat.id);
      setSelectedSeats(updatedSeats);
      
      // Si no quedan asientos seleccionados, liberar la reserva
      if (updatedSeats.length === 0) {
        await releaseSeats();
        setTimeLeft(600); // Reiniciar temporizador
      }
      return;
    }
    
    // Verificar si ya se alcanzó el límite de asientos
    if (selectedSeats.length >= quantity) {
      setError(`Solo puede seleccionar ${quantity} asiento(s).`);
      return;
    }
    
    // Seleccionar nuevo asiento
    try {
      // Si es el primer asiento, iniciar reserva
      if (selectedSeats.length === 0) {
        await ticketService.reserveSeats(eventId, [seat.id]);
        setTimeLeft(600); // 10 minutos
      } else {
        // Agregar a la reserva existente
        await ticketService.reserveSeats(eventId, [seat.id]);
      }
      
      setSelectedSeats([...selectedSeats, seat]);
      setError(null);
    } catch (err) {
      setError('No se pudo reservar el asiento. Puede que ya no esté disponible.');
      console.error('Error reserving seat:', err);
      
      // Actualizar asientos disponibles
      try {
        const seatsData = await eventService.getEventSeats(eventId);
        setSeats(seatsData);
      } catch (error) {
        console.error('Error refreshing seats:', error);
      }
    }
  };

  // Proceder al pago
  const handleProceedToCheckout = async () => {
    if (selectedSeats.length !== quantity) {
      setError(`Por favor, seleccione exactamente ${quantity} asiento(s).`);
      return;
    }
    
    try {
      // Aquí se procesaría la compra de entradas
      const seatIds = selectedSeats.map(seat => seat.id);
      await ticketService.purchaseTickets(eventId, seatIds);
      
      // Redirigir a la página de confirmación
      navigate(`/purchase/confirmation`, { 
        state: { 
          eventId, 
          eventName: event.name,
          seats: selectedSeats,
          quantity,
          totalPrice: event.price * quantity
        } 
      });
    } catch (err) {
      setError('Error al procesar la compra. Por favor, intente nuevamente.');
      console.error('Error purchasing tickets:', err);
    }
  };

  // Renderizar mapa de asientos
  const renderSeatMap = () => {
    if (!seats || seats.length === 0) return null;
    
    // Agrupar asientos por sección
    const sectionGroups = {};
    seats.forEach(seat => {
      if (!sectionGroups[seat.section]) {
        sectionGroups[seat.section] = [];
      }
      sectionGroups[seat.section].push(seat);
    });
    
    return (
      <div className="seat-map-container">
        {Object.keys(sectionGroups).map(section => (
          <div key={section} className="seat-section">
            <h3>Sección {section}</h3>
            <div className="seat-grid">
              {sectionGroups[section].map(seat => {
                const isSelected = selectedSeats.some(s => s.id === seat.id);
                const seatClass = `seat ${seat.status} ${isSelected ? 'selected' : ''}`;
                
                return (
                  <div 
                    key={seat.id} 
                    className={seatClass}
                    onClick={() => handleSeatClick(seat)}
                    title={`Fila ${seat.row}, Asiento ${seat.number}`}
                  >
                    {seat.number}
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    );
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Cargando mapa de asientos...</p>
      </div>
    );
  }

  if (error && selectedSeats.length === 0) {
    return (
      <div className="error-container">
        <h2>Oops!</h2>
        <p>{error}</p>
        <button className="btn btn-primary" onClick={() => navigate(`/events/${eventId}`)}>
          Volver al evento
        </button>
      </div>
    );
  }

  return (
    <div className="seat-selection-page">
      <div className="seat-selection-header">
        <h1>Selección de Asientos</h1>
        <p>Evento: {event?.name}</p>
        {selectedSeats.length > 0 && (
          <div className="reservation-timer">
            <i className="fas fa-clock"></i>
            <span>Tiempo restante: {formatTimeLeft()}</span>
          </div>
        )}
      </div>

      <div className="seat-selection-container">
        <div className="seat-selection-main">
          <div className="seat-map-wrapper">
            <div className="stage">ESCENARIO</div>
            {renderSeatMap()}
            
            <div className="seat-legend">
              <div className="legend-item">
                <div className="seat-example available"></div>
                <span>Disponible</span>
              </div>
              <div className="legend-item">
                <div className="seat-example selected"></div>
                <span>Seleccionado</span>
              </div>
              <div className="legend-item">
                <div className="seat-example occupied"></div>
                <span>Ocupado</span>
              </div>
            </div>
          </div>
          
          {error && (
            <div className="alert alert-danger">
              <i className="fas fa-exclamation-circle"></i>
              {error}
            </div>
          )}
        </div>

        <div className="seat-selection-sidebar">
          <div className="selection-summary">
            <h3>Resumen de selección</h3>
            
            <div className="selected-seats-list">
              <h4>Asientos seleccionados ({selectedSeats.length}/{quantity})</h4>
              {selectedSeats.length > 0 ? (
                <ul>
                  {selectedSeats.map(seat => (
                    <li key={seat.id}>
                      <span>Sección {seat.section}, Fila {seat.row}, Asiento {seat.number}</span>
                      <button 
                        className="remove-seat-btn" 
                        onClick={() => handleSeatClick(seat)}
                        title="Eliminar asiento"
                      >
                        <i className="fas fa-times"></i>
                      </button>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="no-seats-message">No ha seleccionado asientos</p>
              )}
            </div>
            
            <div className="price-summary">
              <div className="summary-row">
                <span>Precio por entrada</span>
                <span>{formatPrice(event?.price)}</span>
              </div>
              <div className="summary-row">
                <span>Cantidad</span>
                <span>{selectedSeats.length}</span>
              </div>
              <div className="summary-row total">
                <span>Total</span>
                <span>{formatPrice(event?.price * selectedSeats.length)}</span>
              </div>
            </div>
            
            <button 
              className="btn btn-primary btn-block btn-lg" 
              onClick={handleProceedToCheckout}
              disabled={selectedSeats.length !== quantity}
            >
              Proceder al pago
            </button>
            
            <button 
              className="btn btn-outline btn-block" 
              onClick={() => navigate(`/events/${eventId}`)}
            >
              Cancelar y volver
            </button>
          </div>
          
          <div className="purchase-info">
            <h4>Información importante</h4>
            <ul>
              <li>Los asientos seleccionados se reservan durante 10 minutos.</li>
              <li>Debe completar la compra antes de que expire el tiempo.</li>
              <li>Una vez realizada la compra, las entradas no son reembolsables.</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SeatSelectionPage;