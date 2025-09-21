import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { eventService } from '../services/eventService';
import { useAuth } from '../context/AuthContext';
import './EventDetailPage.css';

const EventDetailPage = () => {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const fetchEventDetails = async () => {
      try {
        setLoading(true);
        const data = await eventService.getEventById(eventId);
        setEvent(data);
      } catch (err) {
        setError('Error al cargar los detalles del evento. Por favor, intente nuevamente.');
        console.error('Error fetching event details:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchEventDetails();
  }, [eventId]);

  // Formatear fecha
  const formatDate = (dateString) => {
    if (!dateString) return '';
    const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(dateString).toLocaleDateString('es-ES', options);
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

  const handleQuantityChange = (e) => {
    const value = parseInt(e.target.value);
    if (value > 0 && value <= 10) {
      setQuantity(value);
    }
  };

  const handleProceedToSeats = () => {
    if (!isAuthenticated) {
      // Redirigir a login si no está autenticado
      navigate('/login', { state: { from: `/events/${eventId}` } });
      return;
    }
    
    // Redirigir a la selección de asientos
    navigate(`/events/${eventId}/seats`, { state: { quantity } });
  };

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Cargando detalles del evento...</p>
      </div>
    );
  }

  if (error || !event) {
    return (
      <div className="error-container">
        <h2>Oops!</h2>
        <p>{error || 'No se pudo encontrar el evento solicitado.'}</p>
        <button className="btn btn-primary" onClick={() => navigate('/events')}>
          Ver todos los eventos
        </button>
      </div>
    );
  }

  return (
    <div className="event-detail-page">
      {/* Hero section con imagen de fondo */}
      <div 
        className="event-hero" 
        style={{ backgroundImage: `url(${event.imageUrl || 'https://via.placeholder.com/1200x600?text=Evento'})` }}
      >
        <div className="event-hero-overlay">
          <div className="event-hero-content">
            <h1>{event.name}</h1>
            <div className="event-meta">
              <span><i className="fas fa-calendar"></i> {formatDate(event.date)}</span>
              <span><i className="fas fa-map-marker-alt"></i> {event.location}</span>
              <span className="event-category">{event.category}</span>
            </div>
          </div>
        </div>
      </div>

      <div className="event-detail-container">
        <div className="event-detail-content">
          <section className="event-description-section">
            <h2>Descripción</h2>
            <p>{event.description}</p>
          </section>

          <section className="event-details-section">
            <h2>Detalles</h2>
            <div className="event-details-grid">
              <div className="detail-item">
                <h4>Fecha y hora</h4>
                <p>{formatDate(event.date)}</p>
              </div>
              <div className="detail-item">
                <h4>Ubicación</h4>
                <p>{event.location}</p>
                <p>{event.address}</p>
              </div>
              <div className="detail-item">
                <h4>Organizador</h4>
                <p>{event.organizer}</p>
              </div>
              <div className="detail-item">
                <h4>Categoría</h4>
                <p>{event.category}</p>
              </div>
            </div>
          </section>

          {event.additionalInfo && (
            <section className="event-additional-info">
              <h2>Información adicional</h2>
              <p>{event.additionalInfo}</p>
            </section>
          )}
        </div>

        <div className="event-sidebar">
          <div className="ticket-purchase-card">
            <h3>Entradas</h3>
            <div className="ticket-price">
              <span className="price">{formatPrice(event.price)}</span>
              <span className="per-ticket">por entrada</span>
            </div>

            <div className="ticket-quantity">
              <label htmlFor="quantity">Cantidad</label>
              <div className="quantity-selector">
                <button 
                  className="quantity-btn" 
                  onClick={() => quantity > 1 && setQuantity(quantity - 1)}
                  disabled={quantity <= 1}
                >
                  -
                </button>
                <input 
                  type="number" 
                  id="quantity" 
                  value={quantity} 
                  onChange={handleQuantityChange}
                  min="1" 
                  max="10"
                />
                <button 
                  className="quantity-btn" 
                  onClick={() => quantity < 10 && setQuantity(quantity + 1)}
                  disabled={quantity >= 10}
                >
                  +
                </button>
              </div>
            </div>

            <div className="ticket-summary">
              <div className="summary-row">
                <span>Precio por entrada</span>
                <span>{formatPrice(event.price)}</span>
              </div>
              <div className="summary-row">
                <span>Cantidad</span>
                <span>{quantity}</span>
              </div>
              <div className="summary-row total">
                <span>Total</span>
                <span>{formatPrice(event.price * quantity)}</span>
              </div>
            </div>

            <button 
              className="btn btn-primary btn-block btn-lg" 
              onClick={handleProceedToSeats}
            >
              {isAuthenticated ? 'Seleccionar asientos' : 'Iniciar sesión para comprar'}
            </button>

            <div className="ticket-info">
              <p><i className="fas fa-info-circle"></i> Máximo 10 entradas por compra</p>
              <p><i className="fas fa-lock"></i> Pago 100% seguro</p>
            </div>
          </div>

          <div className="share-card">
            <h4>Compartir evento</h4>
            <div className="share-buttons">
              <button className="share-btn facebook">
                <i className="fab fa-facebook-f"></i>
              </button>
              <button className="share-btn twitter">
                <i className="fab fa-twitter"></i>
              </button>
              <button className="share-btn whatsapp">
                <i className="fab fa-whatsapp"></i>
              </button>
              <button className="share-btn email">
                <i className="fas fa-envelope"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EventDetailPage;