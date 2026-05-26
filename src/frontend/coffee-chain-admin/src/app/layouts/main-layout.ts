import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthStore } from '../core/services/auth.store';

interface NavigationItem {
  label: string;
  path: string;
  roles: string[];
}

const NAVIGATION: NavigationItem[] = [
  { label: 'Dashboard', path: '/', roles: ['Administrator', 'BranchManager', 'Cashier'] },
  { label: 'Chi nhanh', path: '/branches', roles: ['Administrator', 'BranchManager'] },
  { label: 'San pham', path: '/products', roles: ['Administrator', 'BranchManager', 'Cashier'] },
  { label: 'Kho hang', path: '/inventory', roles: ['Administrator', 'BranchManager'] },
  { label: 'Nhan vien', path: '/employees', roles: ['Administrator', 'BranchManager'] },
  { label: 'Bao cao', path: '/reports', roles: ['Administrator', 'BranchManager'] },
  { label: 'Khuyen mai', path: '/promotions', roles: ['Administrator', 'BranchManager'] }
];

// MainLayout la shell chinh cua he thong sau khi user dang nhap.
@Component({
  selector: 'ccm-main-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  protected readonly authStore = inject(AuthStore);
  protected readonly sidebarOpen = signal(false);
  protected readonly user = this.authStore.user;
  protected readonly navigation = computed(() => {
    const role = this.authStore.role();
    return NAVIGATION.filter((item) => !role || item.roles.includes(role));
  });

  protected toggleSidebar(): void {
    this.sidebarOpen.update((value) => !value);
  }

  protected closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  protected logout(): void {
    this.authStore.logout();
  }
}
