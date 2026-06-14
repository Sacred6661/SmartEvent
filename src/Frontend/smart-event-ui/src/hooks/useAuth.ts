import { create } from "zustand";
import type { UserInfo, RegisterRequest } from "../types/auth";
import { authService } from "../services/authService";

interface AuthState {
  user: UserInfo | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  isInitialized: boolean;
  login: (email: string, password: string) => Promise<boolean>;
  register: (data: RegisterRequest) => Promise<boolean>;
  loginWithGoogle: () => void;
  logout: () => Promise<void>;
  checkAuth: () => Promise<void>;
}

export const useAuth = create<AuthState>((set, get) => ({
  user: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  isInitialized: false,

  checkAuth: async () => {
    // prevent repeat calls
    if (get().isLoading && get().isInitialized) {
      return;
    }

    try {
      set({ isLoading: true, error: null });

      const response = await authService.getCurrentUser();

      if (response.success && response.data) {
        set({
          user: response.data,
          isAuthenticated: true,
          isLoading: false,
          isInitialized: true,
        });
      } else {
        set({
          user: null,
          isAuthenticated: false,
          isLoading: false,
          isInitialized: true,
        });
      }
    } catch (error: unknown) {
      const axiosError = error as { response?: { status?: number } };

      if (axiosError?.response?.status === 401) {
        set({
          user: null,
          isAuthenticated: false,
          isLoading: false,
          isInitialized: true,
        });
      } else {
        set({
          user: null,
          isAuthenticated: false,
          isLoading: false,
          isInitialized: true,
          error: "Failed to check authentication",
        });
      }
    }
  },

  login: async (email: string, password: string): Promise<boolean> => {
    try {
      set({ isLoading: true, error: null });
      const response = await authService.login({ email, password });

      if (response.success) {
        await new Promise((resolve) => setTimeout(resolve, 100));
        await get().checkAuth();
        return true;
      } else {
        set({
          error: response.message || "Login failed",
          isLoading: false,
        });
        return false;
      }
    } catch {
      set({
        error: "Login failed due to network error",
        isLoading: false,
      });
      return false;
    }
  },

  register: async (data: RegisterRequest): Promise<boolean> => {
    try {
      set({ isLoading: true, error: null });
      const response = await authService.register(data);

      if (response.success) {
        await new Promise((resolve) => setTimeout(resolve, 100));
        await get().checkAuth();
        return true;
      } else {
        set({
          error: response.message || "Registration failed",
          isLoading: false,
        });
        return false;
      }
    } catch {
      set({
        error: "Registration failed due to network error",
        isLoading: false,
      });
      return false;
    }
  },

  loginWithGoogle: () => {
    window.location.href = authService.getGoogleLoginUrl();
  },

  logout: async () => {
    try {
      await authService.logout();
    } finally {
      set({
        user: null,
        isAuthenticated: false,
        isLoading: false,
        error: null,
        isInitialized: true,
      });
    }
  },
}));
