import apiClient from './api';

export const authService = {
  // Matches POST /api/Auth/register
  register: async (email, password, userName) => {
    const response = await apiClient.post('/Auth/register', { email, password, userName });
    return response.data;
  },

  // Matches POST /api/Auth/login
  login: async (userName, password) => {
    const response = await apiClient.post('/Auth/login', { userName, password });
    
    // Assuming .NET Core backend returns an object with a token string
    if (response.data && response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
    }
    return response.data;
  },

  // Simple local logout
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
  }
};