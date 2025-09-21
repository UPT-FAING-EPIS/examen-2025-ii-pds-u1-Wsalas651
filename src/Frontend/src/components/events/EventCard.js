import React from 'react';
import { Link } from 'react-router-dom';
import './EventCard.css';

const EventCard = ({ event }) => {
  // Formatear fecha
  const formatDate = (dateString) => {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('es-ES', options);
  };

  // Formatear precio
  const formatPrice = (price) => {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2
    }).format(price);
  };

  // Obtener categoría formateada
  const getCategoryLabel = (categoryId) => {
    const categories = {
      'conciertos': 'Concierto',
      'teatro': 'Teatro',
      'deportes': 'Deporte',
      'conferencias': 'Conferencia',
      'festivales': 'Festival',
      'otros': 'Otro'
    };
    return categories[categoryId] || 'Evento';
  };

  return (
    <div className="event-card">
      {event.featured && <span className="event-badge featured">Destacado</span>}
      {new Date(event.date) > new Date() && new Date(event.date) < new Date(Date.now() + 7 * 24 * 60 * 60 * 1000) && (
        <span className="event-badge soon">Próximamente</span>
      )}
      
      <div className="event-image">
        <img 
          src={event.imageUrl || 'https://via.placeholder.com/300x200?text=Evento'} 
          alt={event.name} 
        />
        <span className="event-category">{getCategoryLabel(event.category)}</span>
      </div>
      
      <div className="event-content">
        <h3 className="event-title">{event.name}</h3>
        
        <div className="event-details">
          <div className="event-detail">
            <i className="fas fa-calendar"></i>
            <span>{formatDate(event.date)}</span>
          </div>
          
          <div className="event-detail">
            <i className="fas fa-map-marker-alt"></i>
            <span>{event.location}</span>
          </div>
          
          <div className="event-detail">
            <i className="fas fa-ticket-alt"></i>
            <span>Desde {formatPrice(event.price)}</span>
          </div>
        </div>
        
        <p className="event-description">
          {event.description.length > 100
            ? `${event.description.substring(0, 100)}...`
            : event.description}
        </p>
        
        <div className="event-actions">
          <Link to={`/events/${event.id}`} className="btn btn-primary btn-block">
            Ver detalles
          </Link>
        </div>
      </div>
    </div>
  );
};

export default EventCard;