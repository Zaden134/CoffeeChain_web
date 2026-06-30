import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthStore } from '../core/services/auth.store';

interface NavigationItem {
  label: string;
  path: string;
  icon: string;
  roles: string[];
}

const NAVIGATION: NavigationItem[] = [
  { label: 'Bảng điều khiển', path: '/', icon: 'dashboard', roles: ['Administrator', 'BranchManager', 'Cashier', 'WarehouseStaff'] },
  { label: 'Chi nhánh', path: '/branches', icon: 'storefront', roles: ['Administrator', 'BranchManager'] },
  { label: 'Sản phẩm', path: '/products', icon: 'coffee', roles: ['Administrator', 'BranchManager', 'Cashier'] },
  { label: 'Kho hàng', path: '/inventory', icon: 'inventory_2', roles: ['Administrator', 'BranchManager', 'WarehouseStaff'] },
  { label: 'Giao dịch', path: '/inventory/transactions', icon: 'receipt_long', roles: ['Administrator', 'BranchManager', 'WarehouseStaff'] },
  { label: 'Nhân viên', path: '/employees', icon: 'group', roles: ['Administrator', 'BranchManager'] },
  { label: 'Báo cáo', path: '/reports', icon: 'analytics', roles: ['Administrator', 'BranchManager'] },
  { label: 'Khuyến mãi', path: '/promotions', icon: 'local_offer', roles: ['Administrator', 'BranchManager'] },
  { label: 'Tuyển dụng', path: '/recruitment-requests', icon: 'work', roles: ['Administrator', 'BranchManager'] },
  { label: 'Audit log', path: '/audit-logs', icon: 'manage_search', roles: ['Administrator'] }
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
