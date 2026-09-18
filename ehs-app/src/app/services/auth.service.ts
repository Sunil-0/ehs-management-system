import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/Auth.model';

const TOKEN_KEY = 'ehs_token';
const USER_KEY = 'ehs_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // BehaviorSubject holds the "currently logged in user" so any component
  // (like the nav bar) can react immediately when login/logout happens,
  // without re-reading localStorage every time.
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(this.readStoredUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap((response) => {
        // NOTE: localStorage is used here for simplicity in this learning project.
        // It's vulnerable to XSS (any injected script can read it). A production
        // app would typically use an httpOnly cookie set by the server instead,
        // which JavaScript can't read at all. Worth knowing the tradeoff for interviews.
        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(USER_KEY, JSON.stringify(response));
        this.currentUserSubject.next(response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  get currentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    return this.currentUser?.role === role;
  }

  private readStoredUser(): LoginResponse | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}