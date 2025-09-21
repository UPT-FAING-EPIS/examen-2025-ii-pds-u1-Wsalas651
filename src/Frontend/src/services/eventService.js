import { apiClient } from './apiClient';
import supabase from './supabaseClient';

// Determinar si usar Supabase o la API tradicional
const USE_SUPABASE = process.env.REACT_APP_USE_SUPABASE === 'true';

export const eventService = {
  // Obtener todos los eventos con opciones de filtrado
  getEvents: async (params = {}) => {
    try {
      if (USE_SUPABASE) {
        // Implementación con Supabase
        const query = supabase.from('events').select('*');
        
        // Aplicar filtros si existen
        if (params.category) {
          query.eq('category', params.category);
        }
        
        if (params.date) {
          query.gte('date', params.date);
        }
        
        // Ordenar por fecha
        query.order('date', { ascending: true });
        
        const { data, error } = await query;
        if (error) throw error;
        return data;
      } else {
        // Implementación original con API REST
        const response = await apiClient.get('/events', { params });
        return response.data;
      }
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener un evento por su ID
  getEventById: async (id) => {
    try {
      if (USE_SUPABASE) {
        // Implementación con Supabase
        const { data, error } = await supabase
          .from('events')
          .select(`
            *,
            seats(*)
          `)
          .eq('id', id)
          .single();

        if (error) throw error;
        return data;
      } else {
        // Implementación original con API REST
        const response = await apiClient.get(`/events/${id}`);
        return response.data;
      }
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener asientos disponibles para un evento
  getEventSeats: async (eventId) => {
    try {
      if (USE_SUPABASE) {
        // Implementación con Supabase
        const { data, error } = await supabase
          .from('seats')
          .select('*')
          .eq('event_id', eventId);

        if (error) throw error;
        return data;
      } else {
        // Implementación original con API REST
        const response = await apiClient.get(`/events/${eventId}/seats`);
        return response.data;
      }
    } catch (error) {
      throw handleError(error);
    }
  },

  // Buscar eventos por término de búsqueda
  searchEvents: async (searchTerm) => {
    try {
      const response = await apiClient.get('/events/search', {
        params: { query: searchTerm }
      });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener eventos por categoría
  getEventsByCategory: async (category) => {
    try {
      const response = await apiClient.get('/events', {
        params: { category }
      });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener eventos próximos
  getUpcomingEvents: async (limit = 6) => {
    try {
      const response = await apiClient.get('/events/upcoming', {
        params: { limit }
      });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener eventos destacados
  getFeaturedEvents: async () => {
    try {
      const response = await apiClient.get('/events/featured');
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },
};

const handleError = (error) => {
  if (error.response) {
    const errorMessage = error.response.data.message || 'Ha ocurrido un error';
    return new Error(errorMessage);
  } else if (error.request) {
    return new Error('No se pudo conectar con el servidor');
  } else {
    return new Error('Error al procesar la solicitud');
  }
};