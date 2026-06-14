export interface AxiosErrorResponse {
  response?: {
    status?: number;
    data?: {
      message?: string;
      errors?: string[];
    };
  };
  message?: string;
}
