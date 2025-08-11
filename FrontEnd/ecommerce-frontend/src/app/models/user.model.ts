// User model interfaces for the e-commerce application

export interface User {
  id: string;
  email: string;
  userName: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  roles: string[];
  createdAt: Date;
  updatedAt: Date;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  userName: string;
  password: string;
  confirmPassword: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiresIn: number;
  user: User;
}

export interface ApiError {
  message: string;
  errors?: { [key: string]: string[] };
  statusCode: number;
}
