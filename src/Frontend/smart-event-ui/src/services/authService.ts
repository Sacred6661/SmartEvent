import axiosClient from "./axiosClient";
import type {
  ApiResponse,
  UserInfo,
  LoginRequest,
  RegisterRequest,
} from "../types/auth";
import { API_URL } from "../constants/api";

export const authService = {
  login: async (data: LoginRequest): Promise<ApiResponse<UserInfo>> => {
    const response = await axiosClient.post<ApiResponse<UserInfo>>(
      "/auth/login",
      data,
    );
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<ApiResponse<UserInfo>> => {
    const response = await axiosClient.post<ApiResponse<UserInfo>>(
      "/auth/register",
      data,
    );
    return response.data;
  },

  getCurrentUser: async (): Promise<ApiResponse<UserInfo>> => {
    const response = await axiosClient.get<ApiResponse<UserInfo>>("/auth/me");
    return response.data;
  },

  logout: async (): Promise<void> => {
    await axiosClient.post("/auth/logout");
  },

  getGoogleLoginUrl: (): string => {
    return `${API_URL}/auth/google-login`;
  },
};
