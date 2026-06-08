import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthStore } from '../../core/services/auth.store';

// LoginPage dung giao dien tu mau tham chieu va goi backend JWT login that.
@Component({
  selector: 'ccm-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css'
})
export class LoginPage {
  protected readonly authStore = inject(AuthStore);
  private readonly router = inject(Router);

  protected readonly username = signal('admin');
  protected readonly password = signal('Admin@123');
  protected readonly showPassword = signal(false);
  protected readonly error = signal<string | null>(null);

  protected async submit(): Promise<void> {
    this.error.set(null);

    try {
      await this.authStore.login(this.username(), this.password());
      await this.router.navigate(['/']);
    } catch {
      this.error.set('Dang nhap that bai. Kiem tra username va password.');
    }
  }
}
