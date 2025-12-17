import { apiService } from '../shared/apiService';
import {
  LoginRequest,
  RegisterRequest,
  AuthResult,
  ResetPasswordRequest,
  VerificationResult,
  LogoutResult
} from './auth';

export const authService = {
  // Вход в систему
  login: async (credentials: LoginRequest): Promise<AuthResult> => {
    const response = await apiService.post<AuthResult>('/api/v1/auth/login', credentials);
    return response.data;
  },

  // Регистрация
  register: async (userData: RegisterRequest): Promise<AuthResult> => {
    const response = await apiService.post<AuthResult>('/api/v1/auth/register', userData);
    return response.data;
  },

  // Выход
  logout: async (): Promise<LogoutResult> => {
    const response = await apiService.post<LogoutResult>('/api/v1/auth/logout');
    return response.data;
  },

  // Сброс пароля
  resetPassword: async (request: ResetPasswordRequest): Promise<void> => {
    await apiService.post('/api/v1/auth/reset-password', request);
  },

  // Проверка текущего пользователя
  getCurrentUser: async (): Promise<VerificationResult> => {
    const response = await apiService.get<VerificationResult>('/api/v1/auth/me');
    return response.data;
  },

  // Проверка доступности эндпоинта (для проверки подключения)
  healthCheck: async (): Promise<boolean> => {
    try {
      await apiService.get('/api/v1/auth/me');
      return true;
    } catch {
      return false;
    }
  }
};