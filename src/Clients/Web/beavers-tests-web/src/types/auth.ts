export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ResetPasswordRequest {
  email: string;
}

export interface AuthResult {
  success: boolean;
  error?: string;
  idToken?: string;
  refreshToken?: string;
  userId?: string;
  email?: string;
  expiresIn?: string;
}

export interface LogoutResult {
  success: boolean;
  error?: string;
}

export interface UserInfo {
  userId: string;
  email: string;
  displayName: string;
  emailVerified?: boolean;
}

export interface VerificationResult {
  isAuthenticated: boolean;
  userInfo?: UserInfo;
  timestamp: string;
}

export interface ApiError {
  message: string;
  code?: string;
  details?: string;
}