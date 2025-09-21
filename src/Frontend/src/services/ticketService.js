import { apiClient } from './apiClient';

export const ticketService = {
  // Comprar entradas para un evento
  purchaseTickets: async (purchaseData) => {
    try {
      const response = await apiClient.post('/tickets/purchase', purchaseData);
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener entradas del usuario actual
  getUserTickets: async () => {
    try {
      const response = await apiClient.get('/tickets/my-tickets');
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Obtener detalles de una entrada específica
  getTicketById: async (ticketId) => {
    try {
      const response = await apiClient.get(`/tickets/${ticketId}`);
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Verificar disponibilidad de asientos
  checkSeatAvailability: async (eventId, seatIds) => {
    try {
      const response = await apiClient.post(`/events/${eventId}/check-seats`, { seatIds });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Reservar asientos temporalmente durante el proceso de compra
  reserveSeats: async (eventId, seatIds) => {
    try {
      const response = await apiClient.post(`/events/${eventId}/reserve-seats`, { seatIds });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  // Liberar asientos reservados (si el usuario cancela la compra)
  releaseSeats: async (eventId, seatIds) => {
    try {
      const response = await apiClient.post(`/events/${eventId}/release-seats`, { seatIds });
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