import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BranchSummary } from '../../core/models/dashboard.models';
import { EmployeeSummary, UpsertEmployeeRequest } from '../../core/models/employee.models';
import { AuthStore } from '../../core/services/auth.store';
import { BranchApi } from '../../core/services/branch.api';
import { EmployeeApi } from '../../core/services/employee.api';

// EmployeesPage noi vao API CRUD nhan vien va an/hien action theo quyen hien tai.
@Component({
  selector: 'ccm-employees-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employees-page.html',
  styleUrl: './employees-page.css'
})
export class EmployeesPage {
  private readonly employeeApi = inject(EmployeeApi);
  private readonly branchApi = inject(BranchApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly employees = signal<EmployeeSummary[]>([]);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly editingId = signal<string | null>(null);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(5);

  protected readonly form = signal<UpsertEmployeeRequest>({
    username: '',
    fullName: '',
    email: '',
    role: 'Cashier',
    branchId: this.authStore.branchId(),
    password: '',
    isActive: true
  });

  constructor() {
    this.load();
  }

  protected readonly filteredEmployees = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const employees = this.employees();
    if (!term) {
      return employees;
    }

    return employees.filter((employee) =>
      [employee.fullName, employee.username, employee.email, employee.role, employee.branchName]
        .join(' ')
        .toLowerCase()
        .includes(term));
  });

  protected readonly pagedEmployees = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredEmployees().slice(start, start + this.pageSize());
  });

  protected readonly totalPages = computed(() => {
    const total = this.filteredEmployees().length;
    return Math.max(1, Math.ceil(total / this.pageSize()));
  });

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({
      next: (branches) => this.branches.set(branches),
      error: () => this.error.set('Khong tai duoc danh sach chi nhanh.')
    });

    this.employeeApi.getAll().subscribe({
      next: (employees) => {
        this.employees.set(employees);
        this.page.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc danh sach nhan vien.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.editingId.set(null);
    this.page.set(1);
    this.form.set({
      username: '',
      fullName: '',
      email: '',
      role: this.authStore.role() === 'Administrator' ? 'Cashier' : 'Cashier',
      branchId: this.authStore.role() === 'Administrator' ? null : this.authStore.branchId(),
      password: '',
      isActive: true
    });
  }

  protected startEdit(employee: EmployeeSummary): void {
    this.editingId.set(employee.id);
    this.page.set(1);
    this.form.set({
      username: employee.username,
      fullName: employee.fullName,
      email: employee.email,
      role: employee.role,
      branchId: employee.branchId,
      password: '',
      isActive: employee.isActive
    });
  }

  protected async submit(): Promise<void> {
    const payload = this.form();
    this.error.set(this.validate(payload));
    if (this.error()) {
      return;
    }

    this.submitting.set(true);

    const request = this.editingId()
      ? this.employeeApi.update(this.editingId()!, payload)
      : this.employeeApi.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        this.startCreate();
        this.load();
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Khong luu duoc nhan vien.');
      }
    });
  }

  protected delete(employee: EmployeeSummary): void {
    if (!confirm(`Vo hieu hoa nhan vien ${employee.fullName}?`)) {
      return;
    }

    this.employeeApi.delete(employee.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Khong xoa duoc nhan vien.')
    });
  }

  protected canWrite(employee?: EmployeeSummary): boolean {
    const branchId = employee?.branchId ?? this.form().branchId;
    return this.authStore.can('employees.write', branchId);
  }

  protected nextPage(): void {
    this.page.update((value) => Math.min(value + 1, this.totalPages()));
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  protected roleOptions(): string[] {
    return this.authStore.role() === 'Administrator'
      ? ['Administrator', 'BranchManager', 'Cashier', 'WarehouseStaff']
      : ['Cashier', 'WarehouseStaff'];
  }

  private validate(payload: UpsertEmployeeRequest): string | null {
    if (!payload.username.trim() || !payload.fullName.trim() || !payload.email.trim()) {
      return 'Username, ho ten va email la bat buoc.';
    }

    if (!this.editingId() && (!payload.password || payload.password.length < 8)) {
      return 'Mat khau tao moi phai co it nhat 8 ky tu.';
    }

    if (!payload.branchId && payload.role !== 'Administrator') {
      return 'Nhan vien khong phai admin phai gan voi mot chi nhanh.';
    }

    return null;
  }
}
