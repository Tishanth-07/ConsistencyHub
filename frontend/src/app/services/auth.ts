import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  register(payload: { firstname: string; lastname: string; email: string; password: string }) {
    return this.http.post(`${this.api}/api/auth/register`, payload);
  }

  verifyEmail(email: string, code: string) {
    return this.http.post(`${this.api}/api/auth/verify-email`, { email, code });
  }

  resendCode(email: string, purpose: 'verify' | 'reset' = 'verify') {
    return this.http.post(
      `${this.api}/api/auth/resend-code?email=${encodeURIComponent(email)}&purpose=${purpose}`,
      {}
    );
  }

  login(payload: { email: string; password: string }) {
    return this.http.post<{ token: string }>(`${this.api}/api/auth/login`, payload).pipe(
      tap((res) => {
        if (res?.token) this.setToken(res.token);
      })
    );
  }

  forgotPassword(email: string) {
    return this.http.post(`${this.api}/api/auth/forgot-password`, { email });
  }

  resetPassword(email: string, code: string, newPassword: string) {
    return this.http.post(`${this.api}/api/auth/reset-password`, { email, code, newPassword });
  }

  googleLogin(idToken: string) {
    return this.http.post<{ token: string }>(`${this.api}/api/auth/google-login`, { idToken }).pipe(
      tap((res) => {
        if (res?.token) this.setToken(res.token);
      })
    );
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
