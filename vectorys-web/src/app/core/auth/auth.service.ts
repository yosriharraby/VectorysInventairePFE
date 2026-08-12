import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse } from './models/auth-response.model';
import { AuthUser } from './models/auth-user.model';

const STORAGE_KEY = 'vectorys_auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(tap((res) => this.persist(res)));
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): AuthUser | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }
    try {
      const data = JSON.parse(raw) as AuthResponse;
      return { userId: data.userId, nom: data.nom, role: data.role };
    } catch {
      return null;
    }
  }

  getToken(): string | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }
    try {
      const data = JSON.parse(raw) as AuthResponse;
      return data.token ?? null;
    } catch {
      return null;
    }
  }

  getUserRole(): string | null {
    return this.getCurrentUser()?.role ?? null;
  }

  private persist(response: AuthResponse): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
  }
}
