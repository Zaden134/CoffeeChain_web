import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthStore } from '../core/services/auth.store';
import { SupportApi } from '../core/services/support.api';

interface NavigationItem {
  label: string;
  path: string;
  icon: string;
  roles: string[];
}

const NAVIGATION: NavigationItem[] = [
  { label: 'Home', path: '/', icon: 'home', roles: ['Administrator', 'BranchManager', 'Cashier', 'WarehouseStaff'] },
  { label: 'Chi nhánh', path: '/branches', icon: 'storefront', roles: ['Administrator', 'BranchManager'] },
  { label: 'Sản phẩm', path: '/products', icon: 'coffee', roles: ['Administrator', 'BranchManager', 'Cashier'] },
  { label: 'Kho hàng', path: '/inventory', icon: 'inventory_2', roles: ['Administrator', 'BranchManager', 'WarehouseStaff'] },
  { label: 'Giao dịch', path: '/inventory/transactions', icon: 'receipt_long', roles: ['Administrator', 'BranchManager', 'WarehouseStaff'] },
  { label: 'Nhân viên', path: '/employees', icon: 'group', roles: ['Administrator', 'BranchManager'] },
  { label: 'Báo cáo', path: '/reports', icon: 'analytics', roles: ['Administrator', 'BranchManager'] },
  { label: 'Khuyến mãi', path: '/promotions', icon: 'local_offer', roles: ['Administrator', 'BranchManager'] },
  { label: 'Tuyển dụng', path: '/recruitment-requests', icon: 'work', roles: ['Administrator', 'BranchManager'] },
  { label: 'Audit log', path: '/audit-logs', icon: 'manage_search', roles: ['Administrator'] },
  { label: 'Bảo mật', path: '/security', icon: 'admin_panel_settings', roles: ['Administrator', 'BranchManager', 'Cashier', 'WarehouseStaff'] }
];

@Component({
  selector: 'ccm-main-layout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  protected readonly authStore = inject(AuthStore);
  private readonly supportApi = inject(SupportApi);
  protected readonly sidebarOpen = signal(false);
  protected readonly sidebarCollapsed = signal(false);
  protected readonly supportOpen = signal(false);
  protected readonly supportSubmitting = signal(false);
  protected readonly supportMessage = signal<string | null>(null);
  protected readonly supportError = signal<string | null>(null);
  protected readonly supportForm = signal({
    subject: 'Cần hỗ trợ hệ thống',
    message: ''
  });
  protected readonly user = this.authStore.user;
  protected readonly navigation = computed(() => {
    const role = this.authStore.role();
    return NAVIGATION.filter((item) => !role || item.roles.includes(role));
  });

  protected toggleSidebar(): void {
    this.sidebarOpen.update((value) => !value);
  }

  protected toggleSidebarCollapsed(): void {
    this.sidebarCollapsed.update((value) => !value);
  }

  protected closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  protected openSupport(): void {
    this.supportOpen.set(true);
    this.supportMessage.set(null);
    this.supportError.set(null);
  }

  protected closeSupport(): void {
    this.supportOpen.set(false);
  }

  protected submitSupport(): void {
    const payload = this.supportForm();
    if (!payload.subject.trim() || !payload.message.trim()) {
      this.supportError.set('Vui lòng nhập tiêu đề và nội dung cần hỗ trợ.');
      return;
    }

    this.supportSubmitting.set(true);
    this.supportError.set(null);
    this.supportApi.create(payload).subscribe({
      next: () => {
        this.supportSubmitting.set(false);
        this.supportMessage.set('Đã gửi yêu cầu hỗ trợ. Admin có thể xem trong audit log.');
        this.supportForm.set({ subject: 'Cần hỗ trợ hệ thống', message: '' });
      },
      error: () => {
        this.supportSubmitting.set(false);
        this.supportError.set('Không gửi được yêu cầu hỗ trợ. Vui lòng thử lại.');
      }
    });
  }

  protected logout(): void {
    this.authStore.logout();
  }
}
