import { computed, inject, Injectable, signal, Injector } from '@angular/core';
import { Router } from '@angular/router';

import { AuthResponse, UserProfile } from '../models/auth.models';
import { AuthApi } from './auth.api';

export interface AuthState {
  token: string | null;
  refreshToken: string | null;
  user: UserProfile | null;
  expiresAtUtc: string | null;
  refreshTokenExpiresAtUtc: string | null;
  loading: boolean;
}

const TOKEN_KEY = 'ccm.auth.token';
const REFRESH_TOKEN_KEY = 'ccm.auth.refreshToken';
const USER_KEY = 'ccm.auth.user';
const EXPIRES_KEY = 'ccm.auth.expires';
const REFRESH_EXPIRES_KEY = 'ccm.auth.refreshExpires';

// AuthStore giu token, profile va helper auth cho toan bo ung dung.
@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly injector = inject(Injector);
  private get authApi(): AuthApi { return this.injector.get(AuthApi); }
  private readonly router = inject(Router);

  readonly state = signal<AuthState>({
    token: localStorage.getItem(TOKEN_KEY),
    refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY),
    user: this.readUser(),
    expiresAtUtc: localStorage.getItem(EXPIRES_KEY),
    refreshTokenExpiresAtUtc: localStorage.getItem(REFRESH_EXPIRES_KEY),
    loading: false
  });

  readonly token = computed(() => this.state().token);
  readonly refreshToken = computed(() => this.state().refreshToken);
  readonly user = computed(() => this.state().user);
  readonly isAuthenticated = computed(() => {
    const state = this.state();
    if (!state.token || !state.user) return false;
    if (state.expiresAtUtc) {
      const expires = new Date(state.expiresAtUtc).getTime();
      if (Date.now() >= expires) return false;
    }
    return true;
  });
  readonly role = computed(() => this.state().user?.role ?? null);
  readonly branchId = computed(() => this.state().user?.branchId ?? null);

  login(username: string, password: string): Promise<AuthResponse> {
    this.state.update((state) => ({ ...state, loading: true }));

    return new Promise<AuthResponse>((resolve, reject) => {
      this.authApi.login({ username, password }).subscribe({
        next: (response) => {
          this.persist(response);
          resolve(response);
        },
        error: (error: unknown) => {
          this.state.update((state) => ({ ...state, loading: false }));
          reject(error);
        }
      });
    });
  }

  hydrateProfile(): Promise<UserProfile | null> {
    if (!this.state().token && !this.state().refreshToken) {
      return Promise.resolve(null);
    }

    return new Promise<UserProfile | null>((resolve) => {
      this.authApi.me().subscribe({
        next: (user) => {
          const current = this.state();
          this.state.set({ ...current, user });
          localStorage.setItem(USER_KEY, JSON.stringify(user));
          resolve(user);
        },
        error: async () => {
          const refreshed = await this.refreshSession();
          if (!refreshed) {
            this.clear(false);
            resolve(null);
            return;
          }

          this.authApi.me().subscribe({
            next: (user) => {
              const current = this.state();
              this.state.set({ ...current, user });
              localStorage.setItem(USER_KEY, JSON.stringify(user));
              resolve(user);
            },
            error: () => {
              this.clear(false);
              resolve(null);
            }
          });
        }
      });
    });
  }

  logout(): void {
    const refreshToken = this.refreshToken();
    if (refreshToken) {
      void this.authApi.logout({ refreshToken }).subscribe({
        error: () => undefined
      });
    }
    this.clear(true);
  }

  refreshSession(): Promise<AuthResponse | null> {
    const refreshToken = this.refreshToken();
    if (!refreshToken) {
      return Promise.resolve(null);
    }

    this.state.update((state) => ({ ...state, loading: true }));

    return new Promise<AuthResponse | null>((resolve) => {
      this.authApi.refresh({ refreshToken }).subscribe({
        next: (response) => {
          this.persist(response);
          resolve(response);
        },
        error: () => {
          this.clear(true);
          resolve(null);
        }
      });
    });
  }

  can(permission: string, branchId?: string | null): boolean {
    const role = this.role();
    const currentBranchId = this.branchId();

    if (!role) {
      return false;
    }

    if (role === 'Administrator') {
      return true;
    }

    const sameBranch = !branchId || currentBranchId === branchId;

    switch (permission) {
      case 'employees.read':
        return role === 'BranchManager';
      case 'inventory.read':
        return role === 'BranchManager' || role === 'WarehouseStaff';
      case 'recruitment.read':
      case 'promotions.write':
        return role === 'BranchManager';
      case 'employees.write':
        return role === 'BranchManager' && sameBranch;
      case 'inventory.write':
        return (role === 'BranchManager' || role === 'WarehouseStaff') && sameBranch;
      case 'recruitment.write':
        return role === 'BranchManager' && sameBranch;
      case 'recruitment.review':
        return role === 'Administrator';
      case 'promotions.read':
        return role === 'BranchManager' || role === 'Cashier';
      default:
        return false;
    }
  }

  private persist(response: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(EXPIRES_KEY, response.expiresAtUtc);
    localStorage.setItem(REFRESH_EXPIRES_KEY, response.refreshTokenExpiresAtUtc);

    this.state.set({
      token: response.accessToken,
      refreshToken: response.refreshToken,
      user: response.user,
      expiresAtUtc: response.expiresAtUtc,
      refreshTokenExpiresAtUtc: response.refreshTokenExpiresAtUtc,
      loading: false
    });
  }

  private clear(redirectToLogin: boolean): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(EXPIRES_KEY);
    localStorage.removeItem(REFRESH_EXPIRES_KEY);

    this.state.set({
      token: null,
      refreshToken: null,
      user: null,
      expiresAtUtc: null,
      refreshTokenExpiresAtUtc: null,
      loading: false
    });

    if (redirectToLogin) {
      void this.router.navigate(['/login']);
    }
  }

  private readUser(): UserProfile | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as UserProfile) : null;
  }
}
