// User model interface to match the backend API
export interface User {
  id: string;
  fullName: string;
  email: string;
  userName: string;
  emailConfirmed: boolean;
  phoneNumber?: string;
  phoneNumberConfirmed: boolean;
  twoFactorEnabled: boolean;
  lockoutEnabled: boolean;
  accessFailedCount: number;
}

// Authentication request models
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  email: string;
}

// Authentication response models
export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface UserDto {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
}

// Error response model
export interface ApiError {
  message: string;
  errors?: string[];
}
