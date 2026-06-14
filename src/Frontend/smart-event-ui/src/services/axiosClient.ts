import axios, { AxiosError } from "axios";
import type { AxiosErrorResponse } from "../types/errors";

const API_URL = import.meta.env.VITE_API_URL || "https://localhost:7080/api";

const axiosClient = axios.create({
  baseURL: API_URL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

// Response interceptor for 401
axiosClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<AxiosErrorResponse>) => {
    const originalRequest = error.config;

    // If received 401 and this is not a repeat request
    if (
      error.response?.status === 401 &&
      originalRequest &&
      !(originalRequest as { _retry?: boolean })._retry
    ) {
      (originalRequest as { _retry?: boolean })._retry = true;

      try {
        // try to update token using refresh
        // note that we do it in backend middlware first, this is just like backup
        await axios.post(
          `${API_URL}/auth/refresh-token`,
          {},
          {
            withCredentials: true,
          },
        );

        // retry original request
        return axiosClient(originalRequest);
      } catch {
        // If the refresh fails - try again
        console.log("Session expired, redirecting to login...");

        // import dynamically to avoid cyclic dependencies
        const { useAuth } = await import("../hooks/useAuth");
        useAuth.getState().logout();

        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  },
);

export default axiosClient;
