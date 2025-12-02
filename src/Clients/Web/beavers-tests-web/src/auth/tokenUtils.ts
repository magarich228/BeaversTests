import { AuthResult } from "./auth";

// Ключи для localStorage
const TOKEN_KEY = 'auth_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const TOKEN_EXPIRY_KEY = 'token_expiry';

export const tokenUtils = {
  // Сохранение токенов
  setTokens: (authResult: AuthResult) => {
    if (authResult.idToken && authResult.refreshToken) {
      localStorage.setItem(TOKEN_KEY, authResult.idToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, authResult.refreshToken);
      
      // Сохраняем время экспирации (если есть)
      if (authResult.expiresIn) {
        const expiryTime = new Date().getTime() + (parseInt(authResult.expiresIn) * 1000);
        localStorage.setItem(TOKEN_EXPIRY_KEY, expiryTime.toString());
      }
    }
  },

  // Получение токенов
  getToken: (): string | null => localStorage.getItem(TOKEN_KEY),
  getRefreshToken: (): string | null => localStorage.getItem(REFRESH_TOKEN_KEY),

  // Проверка срока действия токена
  isTokenExpired: (): boolean => {
    const expiry = localStorage.getItem(TOKEN_EXPIRY_KEY);
    if (!expiry) return true;
    
    return new Date().getTime() > parseInt(expiry);
  },

  // Очистка токенов
  clearTokens: () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(TOKEN_EXPIRY_KEY);
  },

  // Проверка авторизации
  isAuthenticated: (): boolean => {
    const token = localStorage.getItem(TOKEN_KEY);
    return !!token && !tokenUtils.isTokenExpired();
  }
};