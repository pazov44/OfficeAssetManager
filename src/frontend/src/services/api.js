import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL + '/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Flag to prevent multiple parallel refresh token API requests
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Request Interceptor: Automatically attaches the JWT token if it exists
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response Interceptor: Automatically handles 401 errors using Refresh Token
apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // Check if error is 401 Unauthorized and it hasn't been retried yet
    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      
      // If we are already running a refresh call, queue this request up until it finishes
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return apiClient(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = localStorage.getItem('refreshToken');
      const token = localStorage.getItem('token');

      // If there's no refresh token available locally, break out and force logout
      if (!refreshToken) {
        isRefreshing = false;
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/api/auth/login'; // or login path
        return Promise.reject(error);
      }

      try {
        // Call .NET Core refresh endpoint (adjust '/Auth/refresh' if endpoint path differs)
        // Make sure to use basic 'axios' here instead of 'apiClient' to prevent recursive loop issues
        const response = await axios.post(`${API_BASE_URL}/Auth/refresh`, {
          token: token,
          refreshToken: refreshToken
        });

        if (response.data && response.data.token) {
          // Save the fresh pipeline parameters
          localStorage.setItem('token', response.data.token);
          if (response.data.refreshToken) {
            localStorage.setItem('refreshToken', response.data.refreshToken);
          }

          // Update current failed request header coordinates
          originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
          
          // Clear line for next cycles and release everything in the queue safely
          processQueue(null, response.data.token);
          isRefreshing = false;

          // Retry the original request that failed
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // If the refresh token itself expired or failed, wipe local storage and redirect to login
        processQueue(refreshError, null);
        isRefreshing = false;
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/api/auth/login'; 
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;