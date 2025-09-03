import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RegisterDto {
  username: string;
  password: string;
  role?: string; 
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'http://localhost:5000/api/auth'; 
  constructor(private http: HttpClient) {}

  register(dto: RegisterDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/register`, dto);
  }

  login(dto: LoginDto): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, dto);
  }
}