import apiClient from './api'; // Import interceptor client

export const logService = {
  // GET /api/AssetLog
  getAllActivityLogs: async () => {
    const response = await apiClient.get('/AssetLog');
    return response.data;
  },

  // GET /api/AssetLog/asset/{assetId}
  getLogsForAssetId: async (assetId) => {
    const response = await apiClient.get(`/AssetLog/asset/${assetId}`);
    return response.data;
  }
};