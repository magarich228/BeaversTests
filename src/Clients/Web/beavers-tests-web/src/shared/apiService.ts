import axios, { AxiosInstance, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import { tokenUtils } from '../auth/tokenUtils';
import { AuthResult, RefreshTokenRequest } from '../auth/auth';

const API_BASE_URL = 'http://localhost:5068'; //import.meta.env.VITE_API_URL || 'http://localhost:5068';

class ApiService {
  private client: AxiosInstance;
  private isRefreshing = false;
  private refreshSubscribers: ((token: string) => void)[] = [];

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // TODO: подумать, возможно вынести логику работы с аутентификацией в модуль auth.
    this.setupInterceptors();
  }

  private setupInterceptors(): void {
    // Request interceptor - добавление токена к каждому запросу
    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = tokenUtils.getToken();
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor - обработка ошибок и автоматическое обновление токена
    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      async (error) => {
        const originalRequest = error.config;

        // Если ошибка 401 и это не запрос на обновление токена
        if (error.response?.status === 401 && !originalRequest._retry) {
          if (this.isRefreshing) {
            // Если уже обновляем токен, ждем и повторяем запрос
            return new Promise((resolve) => {
              this.refreshSubscribers.push((token: string) => {
                originalRequest.headers.Authorization = `Bearer ${token}`;
                resolve(this.client(originalRequest));
              });
            });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            const refreshToken = tokenUtils.getRefreshToken();
            if (!refreshToken) {
              throw new Error('No refresh token available');
            }

            const newToken = await this.refreshToken(refreshToken);
            
            // Оповещение всех подписчиков
            this.refreshSubscribers.forEach((callback) => callback(newToken));
            this.refreshSubscribers = [];

            // Повторение оригинального запроса
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
            return this.client(originalRequest);

          } catch (refreshError) {
            // Если не удалось обновить токен - log out пользователя
            tokenUtils.clearTokens();
            window.location.href = '/auth';
            return Promise.reject(refreshError);
          } finally {
            this.isRefreshing = false;
          }
        }

        return Promise.reject(error);
      }
    );
  }

  private async refreshToken(refreshToken: string): Promise<string> {
    const response = await axios.post<AuthResult>(
      `${API_BASE_URL}/api/v1/auth/refresh-token`,
      { refreshToken } as RefreshTokenRequest
    );

    if (response.data.success && response.data.idToken) {
      tokenUtils.setTokens(response.data);
      return response.data.idToken;
    }

    throw new Error('Token refresh failed');
  }

  // Публичные методы для выполнения запросов
  public get = <T>(url: string) => this.client.get<T>(url);
  public post = <T>(url: string, data?: any) => this.client.post<T>(url, data);
  public put = <T>(url: string, data?: any) => this.client.put<T>(url, data);
  public delete = <T>(url: string) => this.client.delete<T>(url);
}

export const apiService = new ApiService();