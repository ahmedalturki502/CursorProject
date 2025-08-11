import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  RegisterRequest, 
  LoginRequest, 
  AuthResponse, 
  UserDto, 
  UpdateProfileRequest,
  ApiError 
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<UserDto | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Check if user is already logged in
    this.loadCurrentUser();
  }

  // Register a new user
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  // Login user
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  // Logout user
  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout`, {})
      .pipe(
        tap(() => this.clearAuth())
      );
  }

  // Get current user profile
  getProfile(): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/profile`)
      .pipe(
        tap(user => this.currentUserSubject.next(user))
      );
  }

  // Update user profile
  updateProfile(request: UpdateProfileRequest): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/profile`, request)
      .pipe(
        tap(user => this.currentUserSubject.next(user))
      );
  }

  // Check if user is authenticated
  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }

  // Check if user has admin role
  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.roles?.includes('Admin') || false;
  }

  // Get current user
  getCurrentUser(): UserDto | null {
    return this.currentUserSubject.value;
  }

  // Get auth token
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  // Set auth token
  private setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  // Clear auth data
  private clearAuth(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('refresh_token');
    this.currentUserSubject.next(null);
  }

  // Handle authentication response
  private handleAuthResponse(response: AuthResponse): void {
    this.setToken(response.token);
    localStorage.setItem('refresh_token', response.refreshToken);
    this.currentUserSubject.next(response.user);
  }

  // Load current user from token
  private loadCurrentUser(): void {
    if (this.isAuthenticated()) {
      this.getProfile().subscribe({
        error: () => this.clearAuth()
      });
    }
  }

  // Check if token is expired
  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }
}
