import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserLogin, UserRegister, AuthResult } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7271/api/auth'; 

  constructor(private httpClient: HttpClient) { }

  register(registerData: UserRegister): Observable<AuthResult> {
    return this.httpClient.post<AuthResult>(`${this.apiUrl}/register`, registerData);
  }

  login(loginData: UserLogin): Observable<AuthResult> {
    return this.httpClient.post<AuthResult>(`${this.apiUrl}/login`, loginData)
      .pipe(
        tap(response => {
          if (response.succeeded && response.token) {
            localStorage.setItem('authToken', response.token);
             console.log('Token stored in localStorage');
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
     console.log('Token removed from localStorage');
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('authToken');
  }

}