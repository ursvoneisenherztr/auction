import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, of } from 'rxjs';
import { User, LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:7071/api';
  private currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromStorage());
  private tokenSubject = new BehaviorSubject<string | null>(this.getTokenFromStorage());

  public currentUser$ = this.currentUserSubject.asObservable();
  public token$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {
    this.checkTokenExpiry();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        this.setToken(response.token);
        this.setCurrentUser(response.user);
      }),
      catchError(error => {
        console.error('Login failed:', error);
        throw error;
      })
    );
  }

  register(data: RegisterRequest): Observable<LoginResponse> {
    const { confirmPassword, ...registerData } = data;
    return this.http.post<LoginResponse>(`${this.apiUrl}/register`, registerData).pipe(
      tap(response => {
        this.setToken(response.token);
        this.setCurrentUser(response.user);
      }),
      catchError(error => {
        console.error('Registration failed:', error);
        throw error;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    this.currentUserSubject.next(null);
    this.tokenSubject.next(null);
  }

  isAuthenticated(): boolean {
    return this.getTokenFromStorage() !== null && this.isTokenValid();
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === 'admin';
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return this.getTokenFromStorage();
  }

  private setToken(token: string): void {
    localStorage.setItem('auth_token', token);
    this.tokenSubject.next(token);
  }

  private setCurrentUser(user: User): void {
    localStorage.setItem('auth_user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getTokenFromStorage(): string | null {
    return localStorage.getItem('auth_token');
  }

  private getUserFromStorage(): User | null {
    const userJson = localStorage.getItem('auth_user');
    return userJson ? JSON.parse(userJson) : null;
  }

  private isTokenValid(): boolean {
    const token = this.getTokenFromStorage();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;
      return Date.now() < expirationTime;
    } catch {
      return false;
    }
  }

  private checkTokenExpiry(): void {
    setInterval(() => {
      if (!this.isTokenValid()) {
        this.logout();
      }
    }, 60000); // Check every minute
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getTokenFromStorage();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }
}
