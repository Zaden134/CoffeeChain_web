import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BranchSummary } from '../../core/models/dashboard.models';
import { BranchApi, UpsertBranchRequest } from '../../core/services/branch.api';
import { AuthStore } from '../../core/services/auth.store';

// BranchesPage hien thi danh sach chi nhanh va KPI co ban tu backend.
@Component({
  selector: 'ccm-branches-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './branches-page.html',
  styleUrl: './branches-page.css'
})
export class BranchesPage {
  private readonly branchApi = inject(BranchApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly success = signal<string | null>(null);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly editingBranch = signal<BranchSummary | null>(null);
  protected readonly form = signal<UpsertBranchRequest>({
    code: '',
    name: '',
    address: '',
    managerName: '',
    isActive: true
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({
      next: (branches) => {
        this.branches.set(branches);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc danh sach chi nhanh.');
        this.loading.set(false);
      }
    });
  }

  protected canEdit(branch: BranchSummary): boolean {
    const role = this.authStore.role();
    return role === 'Administrator' || (role === 'BranchManager' && this.authStore.branchId() === branch.id);
  }

  protected startEdit(branch: BranchSummary): void {
    this.error.set(null);
    this.success.set(null);
    this.editingBranch.set(branch);
    this.form.set({
      code: branch.code,
      name: branch.name,
      address: branch.address,
      managerName: branch.managerName,
      isActive: branch.isActive
    });
  }

  protected cancelEdit(): void {
    this.editingBranch.set(null);
    this.error.set(null);
  }

  protected save(): void {
    const branch = this.editingBranch();
    if (!branch) {
      return;
    }

    const payload = this.form();
    if (!payload.code.trim() || !payload.name.trim() || !payload.address.trim() || !payload.managerName.trim()) {
      this.error.set('Vui lòng nhập đầy đủ mã, tên, địa chỉ và quản lý chi nhánh.');
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.success.set(null);
    this.branchApi.update(branch.id, payload).subscribe({
      next: (updated) => {
        this.branches.update((branches) => branches.map((item) => item.id === updated.id ? updated : item));
        this.editingBranch.set(updated);
        this.form.set({
          code: updated.code,
          name: updated.name,
          address: updated.address,
          managerName: updated.managerName,
          isActive: updated.isActive
        });
        this.success.set('Đã cập nhật chi nhánh.');
        this.saving.set(false);
      },
      error: (error) => {
        this.error.set(error?.error?.message ?? 'Không cập nhật được chi nhánh.');
        this.saving.set(false);
      }
    });
  }
}
