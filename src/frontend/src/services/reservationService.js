import apiClient from './api'; // Import interceptor client

export const reservationService = {
  // POST /api/Reservation
  createReservation: async (payload) => {
    const response = await apiClient.post('/Reservation', payload);
    return response.data;
  },

  // GET /api/Reservation
  getAllReservations: async () => {
    const response = await apiClient.get('/Reservation');
    return response.data;
  },

  // GET /api/Reservation/my-reservations
  getMyReservations: async () => {
    const response = await apiClient.get('/Reservation/my-reservations');
    return response.data;
  },

  // PATCH /api/Reservation/{id}/status
  updateStatus: async (id, statusString) => {
    // We explicitly stringify this because your swagger requires a raw JSON string property
    const response = await apiClient.patch(
      `/Reservation/${id}/status`,
      JSON.stringify(statusString)
    );
    return response.data;
  },

  // DELETE /api/Reservation/{id}/cancel
  cancelReservation: async (id) => {
    const response = await apiClient.delete(`/Reservation/${id}/cancel`);
    return response.data;
  }
};