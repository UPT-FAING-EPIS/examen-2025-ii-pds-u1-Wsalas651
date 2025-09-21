import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { eventService } from '../services/eventService';
import EventCard from '../components/events/EventCard';
import './EventsPage.css';

const EventsPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Filtros
  const [searchTerm, setSearchTerm] = useState(searchParams.get('query') || '');
  const [selectedCategory, setSelectedCategory] = useState(searchParams.get('category') || '');
  const [dateFilter, setDateFilter] = useState(searchParams.get('date') || '');
  const [priceRange, setPriceRange] = useState({
    min: searchParams.get('minPrice') || '',
    max: searchParams.get('maxPrice') || ''
  });

  // Categorías disponibles
  const categories = [
    { id: 'conciertos', name: 'Conciertos' },
    { id: 'teatro', name: 'Teatro' },
    { id: 'deportes', name: 'Deportes' },
    { id: 'conferencias', name: 'Conferencias' },
    { id: 'festivales', name: 'Festivales' },
    { id: 'otros', name: 'Otros' }
  ];

  useEffect(() => {
    // Actualizar los filtros cuando cambian los parámetros de URL
    setSearchTerm(searchParams.get('query') || '');
    setSelectedCategory(searchParams.get('category') || '');
    setDateFilter(searchParams.get('date') || '');
    setPriceRange({
      min: searchParams.get('minPrice') || '',
      max: searchParams.get('maxPrice') || ''
    });
  }, [searchParams]);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        setLoading(true);
        
        // Construir parámetros de filtrado directamente desde searchParams
        const params = {};
        const query = searchParams.get('query');
        const category = searchParams.get('category');
        const date = searchParams.get('date');
        const minPrice = searchParams.get('minPrice');
        const maxPrice = searchParams.get('maxPrice');
        
        if (query) params.query = query;
        if (category) params.category = category;
        if (date) params.date = date;
        if (minPrice) params.minPrice = minPrice;
        if (maxPrice) params.maxPrice = maxPrice;
        
        const data = await eventService.getEvents(params);
        setEvents(data);
      } catch (err) {
        setError('Error al cargar los eventos. Por favor, intente nuevamente.');
        console.error('Error fetching events:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, [searchParams]);

  const handleSearch = (e) => {
    e.preventDefault();
    
    // Actualizar parámetros de URL con los filtros
    const params = {};
    if (searchTerm) params.query = searchTerm;
    if (selectedCategory) params.category = selectedCategory;
    if (dateFilter) params.date = dateFilter;
    if (priceRange.min) params.minPrice = priceRange.min;
    if (priceRange.max) params.maxPrice = priceRange.max;
    
    setSearchParams(params);
  };

  const clearFilters = () => {
    setSearchTerm('');
    setSelectedCategory('');
    setDateFilter('');
    setPriceRange({ min: '', max: '' });
    setSearchParams({});
  };

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
    <div className="events-page">
      <div className="events-header">
        <h1>Eventos</h1>
        <p>Encuentra y compra entradas para los mejores eventos</p>
      </div>

      <div className="events-container">
        {/* Sidebar con filtros */}
        <aside className="filters-sidebar">
          <h3>Filtros</h3>
          <form onSubmit={handleSearch}>
            {/* Búsqueda por nombre */}
            <div className="filter-group">
              <label htmlFor="search">Buscar</label>
              <input
                type="text"
                id="search"
                placeholder="Nombre del evento"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="form-control"
              />
            </div>

            {/* Filtro por categoría */}
            <div className="filter-group">
              <label htmlFor="category">Categoría</label>
              <select
                id="category"
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value)}
                className="form-control"
              >
                <option value="">Todas las categorías</option>
                {categories.map(category => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Filtro por fecha */}
            <div className="filter-group">
              <label htmlFor="date">Fecha</label>
              <select
                id="date"
                value={dateFilter}
                onChange={(e) => setDateFilter(e.target.value)}
                className="form-control"
              >
                <option value="">Cualquier fecha</option>
                <option value="today">Hoy</option>
                <option value="tomorrow">Mañana</option>
                <option value="this-week">Esta semana</option>
                <option value="this-weekend">Este fin de semana</option>
                <option value="next-week">Próxima semana</option>
                <option value="this-month">Este mes</option>
              </select>
            </div>

            {/* Filtro por rango de precio */}
            <div className="filter-group">
              <label>Rango de precio</label>
              <div className="price-range">
                <input
                  type="number"
                  placeholder="Min"
                  value={priceRange.min}
                  onChange={(e) => setPriceRange({ ...priceRange, min: e.target.value })}
                  className="form-control"
                  min="0"
                />
                <span>-</span>
                <input
                  type="number"
                  placeholder="Max"
                  value={priceRange.max}
                  onChange={(e) => setPriceRange({ ...priceRange, max: e.target.value })}
                  className="form-control"
                  min="0"
                />
              </div>
            </div>

            {/* Botones de acción */}
            <div className="filter-actions">
              <button type="submit" className="btn btn-primary btn-block">
                Aplicar Filtros
              </button>
              <button type="button" className="btn btn-outline btn-block" onClick={clearFilters}>
                Limpiar Filtros
              </button>
            </div>
          </form>
        </aside>

        {/* Lista de eventos */}
        <div className="events-list">
          {events.length > 0 ? (
            <>
              <div className="events-count">
                <p>Mostrando {events.length} eventos</p>
              </div>
              <div className="events-grid">
                {events.map(event => (
                  <EventCard key={event.id} event={event} />
                ))}
              </div>
            </>
          ) : (
            <div className="no-events">
              <h3>No se encontraron eventos</h3>
              <p>Intenta con otros filtros o vuelve más tarde.</p>
              <button className="btn btn-primary" onClick={clearFilters}>
                Ver todos los eventos
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default EventsPage;