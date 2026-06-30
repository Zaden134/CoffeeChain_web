import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { AuthResponse, ChangePasswordRequest, LoginRequest, ResetPasswordRequest, UserProfile, UserSession, UserSessionQuery } from '../models/auth.models';
import { PagedResult } from '../models/paged.models';

interface RefreshRequest {
  refreshToken: string;
}

// AuthApi tap trung giao tiep voi endpoint auth cua backend.
@Injectable({ providedIn: 'root' })
export class AuthApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  login(payload: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, payload);
  }

  refresh(payload: RefreshRequest) {
    return this.http.post<AuthResponse>(`${this.baseUrl}/refresh`, payload);
  }

  logout(payload: RefreshRequest) {
    return this.http.post<void>(`${this.baseUrl}/logout`, payload);
  }

  me() {
    return this.http.get<UserProfile>(`${this.baseUrl}/me`);
  }

  changePassword(payload: ChangePasswordRequest) {
    return this.http.post<void>(`${this.baseUrl}/change-password`, payload);
  }

  resetPassword(employeeId: string, payload: ResetPasswordRequest) {
    return this.http.post<void>(`${this.baseUrl}/users/${employeeId}/reset-password`, payload);
  }

  getSessions(query: UserSessionQuery) {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
      activeOnly: String(query.activeOnly ?? false)
    };

    if (query.search?.trim()) {
      params['search'] = query.search.trim();
    }

    return this.http.get<PagedResult<UserSession>>(`${this.baseUrl}/sessions`, { params });
  }

  revokeSession(sessionId: string) {
    return this.http.post<void>(`${this.baseUrl}/sessions/${sessionId}/revoke`, {});
  }
}
