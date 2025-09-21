import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { eventService } from '../services/eventService';
import './HomePage.css';

const HomePage = () => {
  const [featuredEvents, setFeaturedEvents] = useState([]);
  const [upcomingEvents, setUpcomingEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        setLoading(true);
        const [featuredData, upcomingData] = await Promise.all([
          eventService.getFeaturedEvents(),
          eventService.getUpcomingEvents(6)
        ]);
        
        setFeaturedEvents(featuredData);
        setUpcomingEvents(upcomingData);
      } catch (err) {
        setError('Error al cargar los eventos. Por favor, intente nuevamente.');
        console.error('Error fetching events:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  if (loading) {
    return (
      <div className="loading-container">
        <div className="spinner"></div>
        <p>Cargando eventos...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-container">
        <h2>Oops!</h2>
        <p>{error}</p>
        <button className="btn btn-primary" onClick={() => window.location.reload()}>
          Reintentar
        </button>
      </div>
    );
  }

  return (
    <div className="home-page">
      {/* Hero Section */}
      <section className="hero-section">
        <div className="hero-content">
          <h1>Encuentra los mejores eventos</h1>
          <p>Compra entradas para conciertos, teatro, deportes y más</p>
          <Link to="/events" className="btn btn-primary btn-lg">
            Explorar Eventos
          </Link>
        </div>
      </section>

      {/* Featured Events Section */}
      <section className="featured-events-section">
        <div className="section-header">
          <h2>Eventos Destacados</h2>
          <Link to="/events" className="view-all-link">Ver todos</Link>
        </div>
        
        <div className="events-grid">
          {featuredEvents.length > 0 ? (
            featuredEvents.map(event => (
              <div className="event-card" key={event.id}>
                <div className="event-image">
                  <img src={event.imageUrl || 'https://via.placeholder.com/300x200?text=Evento'} alt={event.name} />
                  {event.isSoldOut && <span className="sold-out-badge">Agotado</span>}
                </div>
                <div className="event-details">
                  <h3 className="event-title">{event.name}</h3>
                  <p className="event-date">{new Date(event.date).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })}</p>
                  <p className="event-location">{event.location}</p>
                  <p className="event-price">Desde ${event.basePrice}</p>
                  <Link to={`/events/${event.id}`} className="btn btn-outline btn-block">
                    Ver Detalles
                  </Link>
                </div>
              </div>
            ))
          ) : (
            <p className="no-events-message">No hay eventos destacados disponibles actualmente.</p>
          )}
        </div>
      </section>

      {/* Upcoming Events Section */}
      <section className="upcoming-events-section">
        <div className="section-header">
          <h2>Próximos Eventos</h2>
          <Link to="/events" className="view-all-link">Ver todos</Link>
        </div>
        
        <div className="events-grid">
          {upcomingEvents.length > 0 ? (
            upcomingEvents.map(event => (
              <div className="event-card" key={event.id}>
                <div className="event-image">
                  <img src={event.imageUrl || 'https://via.placeholder.com/300x200?text=Evento'} alt={event.name} />
                  {new Date(event.date) < new Date(Date.now() + 7 * 24 * 60 * 60 * 1000) && (
                    <span className="soon-badge">¡Pronto!</span>
                  )}
                </div>
                <div className="event-details">
                  <h3 className="event-title">{event.name}</h3>
                  <p className="event-date">{new Date(event.date).toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' })}</p>
                  <p className="event-location">{event.location}</p>
                  <p className="event-price">Desde ${event.basePrice}</p>
                  <Link to={`/events/${event.id}`} className="btn btn-outline btn-block">
                    Ver Detalles
                  </Link>
                </div>
              </div>
            ))
          ) : (
            <p className="no-events-message">No hay próximos eventos disponibles actualmente.</p>
          )}
        </div>
      </section>

      {/* Categories Section */}
      <section className="categories-section">
        <h2>Explora por Categorías</h2>
        <div className="categories-grid">
          <Link to="/events?category=conciertos" className="category-card">
            <div className="category-icon">
              <i className="fas fa-music"></i>
            </div>
            <h3>Conciertos</h3>
          </Link>
          
          <Link to="/events?category=teatro" className="category-card">
            <div className="category-icon">
              <i className="fas fa-theater-masks"></i>
            </div>
            <h3>Teatro</h3>
          </Link>
          
          <Link to="/events?category=deportes" className="category-card">
            <div className="category-icon">
              <i className="fas fa-futbol"></i>
            </div>
            <h3>Deportes</h3>
          </Link>
          
          <Link to="/events?category=conferencias" className="category-card">
            <div className="category-icon">
              <i className="fas fa-microphone"></i>
            </div>
            <h3>Conferencias</h3>
          </Link>
        </div>
      </section>

      {/* Newsletter Section */}
      <section className="newsletter-section">
        <div className="newsletter-content">
          <h2>Mantente Informado</h2>
          <p>Suscríbete para recibir notificaciones sobre nuevos eventos y ofertas especiales</p>
          <form className="newsletter-form">
            <input type="email" placeholder="Tu correo electrónico" required />
            <button type="submit" className="btn btn-primary">Suscribirse</button>
          </form>
        </div>
      </section>
    </div>
  );
};

export default HomePage;