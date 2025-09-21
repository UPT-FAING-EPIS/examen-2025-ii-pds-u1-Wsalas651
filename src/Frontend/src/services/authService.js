import { apiClient } from './apiClient';

export const authService = {
  login: async (email, password) => {
    try {
      const response = await apiClient.post('/auth/login', { email, password });
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  register: async (userData) => {
    try {
      const response = await apiClient.post('/auth/register', userData);
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  getCurrentUser: async () => {
    try {
      const response = await apiClient.get('/users/me');
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  updateProfile: async (userData) => {
    try {
      const response = await apiClient.put('/users/profile', userData);
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },

  changePassword: async (passwordData) => {
    try {
      const response = await apiClient.put('/users/change-password', passwordData);
      return response.data;
    } catch (error) {
      throw handleError(error);
    }
  },
};

const handleError = (error) => {
  if (error.response) {
    // El servidor respondió con un código de estado fuera del rango 2xx
    const errorMessage = error.response.data.message || 'Ha ocurrido un error';
    return new Error(errorMessage);
  } else if (error.request) {
    // La solicitud fue hecha pero no se recibió respuesta
    return new Error('No se pudo conectar con el servidor');
  } else {
    // Algo sucedió en la configuración de la solicitud que desencadenó un error
    return new Error('Error al procesar la solicitud');
  }
};