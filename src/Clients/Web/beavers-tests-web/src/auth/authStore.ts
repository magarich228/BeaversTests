import { makeAutoObservable, runInAction } from 'mobx';
import { authService } from './authService';
import { tokenUtils } from './tokenUtils';
import {
  LoginRequest,
  RegisterRequest,
  AuthResult,
  UserInfo,
  VerificationResult
} from './auth'

class AuthStore {
  // Состояние хранилища
  user: UserInfo | null = null;
  isAuthenticated = false;
  isLoading = false;
  error: string | null = null;

  constructor() {
    makeAutoObservable(this);
    this.initializeAuth();
  }

  // Инициализация авторизации при загрузке приложения
  initializeAuth = async (): Promise<void> => {
    const token = tokenUtils.getToken();
    if (token && !tokenUtils.isTokenExpired()) {
      await this.checkAuth();
    } else if (tokenUtils.isTokenExpired()) {
      tokenUtils.clearTokens();
    }
  };

  // Проверка авторизации
  checkAuth = async (): Promise<boolean> => {
    try {
      this.setLoading(true);
      const result: VerificationResult = await authService.getCurrentUser();
      
      runInAction(() => {
        if (result.isAuthenticated && result.userInfo) {
          this.user = result.userInfo;
          this.isAuthenticated = true;
          this.error = null;
        } else {
          this.clearAuth();
        }
      });

      return result.isAuthenticated;
    } catch (error) {
      runInAction(() => {
        this.clearAuth();
        this.error = this.getErrorMessage(error);
      });
      return false;
    } finally {
      runInAction(() => {
        this.setLoading(false);
      });
    }
  };

  // Вход в систему
  login = async (credentials: LoginRequest): Promise<boolean> => {
    try {
      this.setLoading(true);
      this.clearError();

      const result: AuthResult = await authService.login(credentials);

      runInAction(() => {
        if (result.success) {
          // Сохраняем токены
          tokenUtils.setTokens(result);
          
          // Устанавливаем пользователя
          if (result.userId && result.email) {
            this.user = {
              userId: result.userId,
              email: result.email,
              displayName: result.email.split('@')[0] // Временное имя
            };
            this.isAuthenticated = true;
          }
          this.error = null;
        } else {
          this.error = result.error || 'Login failed';
        }
      });

      return result.success;
    } catch (error) {
      runInAction(() => {
        this.error = this.getErrorMessage(error);
      });
      return false;
    } finally {
      runInAction(() => {
        this.setLoading(false);
      });
    }
  };

  // Регистрация
  register = async (userData: RegisterRequest): Promise<boolean> => {
    try {
      this.setLoading(true);
      this.clearError();

      const result: AuthResult = await authService.register(userData);

      runInAction(() => {
        if (result.success) {
          // После успешной регистрации автоматически логинимся
          if (result.idToken && result.refreshToken) {
            tokenUtils.setTokens(result);
            
            if (result.userId && result.email) {
              this.user = {
                userId: result.userId,
                email: result.email,
                displayName: userData.displayName
              };
              this.isAuthenticated = true;
            }
          }
          this.error = null;
        } else {
          this.error = result.error || 'Registration failed';
        }
      });

      return result.success;
    } catch (error) {
      runInAction(() => {
        this.error = this.getErrorMessage(error);
      });
      return false;
    } finally {
      runInAction(() => {
        this.setLoading(false);
      });
    }
  };

  // Выход
  logout = async (): Promise<void> => {
    try {
      if (this.isAuthenticated) {
        await authService.logout();
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      runInAction(() => {
        this.clearAuth();
        tokenUtils.clearTokens();
      });
    }
  };

  // Сброс пароля
  resetPassword = async (email: string): Promise<boolean> => {
    try {
      this.setLoading(true);
      await authService.resetPassword({ email });
      return true;
    } catch (error) {
      runInAction(() => {
        this.error = this.getErrorMessage(error);
      });
      return false;
    } finally {
      runInAction(() => {
        this.setLoading(false);
      });
    }
  };

  // Очистка ошибок
  clearError = (): void => {
    this.error = null;
  };

  // Установка состояния загрузки
  private setLoading = (loading: boolean): void => {
    this.isLoading = loading;
  };

  // Очистка авторизации
  private clearAuth = (): void => {
    this.user = null;
    this.isAuthenticated = false;
    this.error = null;
  };

  // Преобразование ошибки в читаемое сообщение
  private getErrorMessage = (error: any): string => {
    // TODO: вынести в отдельный тип?
    if (error.response?.data?.error) {
      return error.response.data.error;
    }
    
    if (error.response?.status === 401) {
      return 'Invalid email or password';
    }
    
    if (error.response?.status === 400) {
      return 'Email not verified. Please check your email for verification link.';
    }
    
    if (error.message) {
      return error.message;
    }
    
    return 'An unexpected error occurred';
  };
}

export default AuthStore;