import apiClient from './api'; // Import interceptor client instead of raw axios

export const assetService = {
  // GET /api/Asset
  getAllAssets: async () => {
    const response = await apiClient.get('/Asset');
    return response.data;
  },

  // POST /api/Asset
  createAsset: async (assetData) => {
    const response = await apiClient.post('/Asset', assetData);
    return response.data;
  },

  // GET /api/Asset/{id}
  getAssetById: async (id) => {
    const response = await apiClient.get(`/Asset/${id}`);
    return response.data;
  },

  // PUT /api/Asset/{id}
  updateAsset: async (id, updateData) => {
    const response = await apiClient.put(`/Asset/${id}`, updateData);
    return response.data;
  },

  // DELETE /api/Asset/{id}
  deleteAsset: async (id) => {
    const response = await apiClient.delete(`/Asset/${id}`);
    return response.data;
  },

  // GET /api/Asset/{id}/logs
  getLogsByAssetId: async (id) => {
    const response = await apiClient.get(`/Asset/${id}/logs`);
    return response.data;
  }
};