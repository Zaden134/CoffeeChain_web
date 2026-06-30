import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { UserSession } from '../../core/models/auth.models';
import { PagedResult } from '../../core/models/paged.models';
import { AuthStore } from '../../core/services/auth.store';
import { AuthApi } from '../../core/services/auth.api';

@Component({
  selector: 'ccm-security-page',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule],
  templateUrl: './security-page.html',
  styleUrl: './security-page.css'
})
export class SecurityPage {
  private readonly authApi = inject(AuthApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly currentPassword = signal('');
  protected readonly newPassword = signal('');
  protected readonly confirmPassword = signal('');
  protected readonly resetPassword = signal('');
  protected readonly selectedEmployeeId = signal<string | null>(null);
  protected readonly message = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);
  protected readonly submitting = signal(false);

  protected readonly sessionSearch = signal('');
  protected readonly activeOnly = signal(false);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(20);
  protected readonly sessions = signal<PagedResult<UserSession> | null>(null);

  constructor() {
    if (this.isAdmin()) {
      this.loadSessions();
    }
  }

  protected isAdmin(): boolean {
    return this.authStore.role() === 'Administrator';
  }

  protected changePassword(): void {
    this.error.set(null);
    this.message.set(null);

    if (this.newPassword().length < 8 || this.newPassword() !== this.confirmPassword()) {
      this.error.set('Mật khẩu mới phải có ít nhất 8 ký tự và khớp xác nhận.');
      return;
    }

    this.submitting.set(true);
    this.authApi.changePassword({
      currentPassword: this.currentPassword(),
      newPassword: this.newPassword()
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.currentPassword.set('');
        this.newPassword.set('');
        this.confirmPassword.set('');
        this.message.set('Đổi mật khẩu thành công. Các phiên cũ đã bị thu hồi.');
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Không đổi được mật khẩu.');
      }
    });
  }

  protected loadSessions(): void {
    this.authApi.getSessions({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.sessionSearch(),
      activeOnly: this.activeOnly()
    }).subscribe({
      next: (result) => this.sessions.set(result),
      error: () => this.error.set('Không tải được danh sách phiên đăng nhập.')
    });
  }

  protected changeSessionSearch(value: string): void {
    this.sessionSearch.set(value);
    this.page.set(1);
    this.loadSessions();
  }

  protected toggleActiveOnly(value: boolean): void {
    this.activeOnly.set(value);
    this.page.set(1);
    this.loadSessions();
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
    this.loadSessions();
  }

  protected nextPage(): void {
    this.page.update((value) => Math.min(value + 1, this.sessions()?.totalPages ?? 1));
    this.loadSessions();
  }

  protected revoke(session: UserSession): void {
    this.authApi.revokeSession(session.id).subscribe({
      next: () => {
        this.message.set('Đã thu hồi phiên đăng nhập.');
        this.loadSessions();
      },
      error: (error) => this.error.set(error?.error?.message ?? 'Không thu hồi được phiên.')
    });
  }

  protected selectReset(session: UserSession): void {
    this.selectedEmployeeId.set(session.employeeId);
    this.resetPassword.set('');
  }

  protected submitReset(): void {
    const employeeId = this.selectedEmployeeId();
    if (!employeeId || this.resetPassword().length < 8) {
      this.error.set('Mật khẩu reset phải có ít nhất 8 ký tự.');
      return;
    }

    this.authApi.resetPassword(employeeId, { newPassword: this.resetPassword() }).subscribe({
      next: () => {
        this.message.set('Đã reset mật khẩu và thu hồi các phiên của user.');
        this.selectedEmployeeId.set(null);
        this.resetPassword.set('');
        this.loadSessions();
      },
      error: (error) => this.error.set(error?.error?.message ?? 'Không reset được mật khẩu.')
    });
  }
}
