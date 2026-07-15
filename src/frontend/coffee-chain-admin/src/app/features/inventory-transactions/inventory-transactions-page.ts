import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AuthStore } from '../../core/services/auth.store';
import { BranchApi } from '../../core/services/branch.api';
import { InventoryApi } from '../../core/services/inventory.api';
import { InventoryTransactionApi, InventoryTransactionDto, CreateInventoryTransactionRequestDto } from '../../core/services/inventory-transaction.api';
import { BranchSummary } from '../../core/models/dashboard.models';
import { LookupItem } from '../../core/models/common.models';

@Component({
  selector: 'ccm-inventory-transactions-page',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './inventory-transactions-page.html',
  styleUrl: './inventory-transactions-page.css'
})
export class InventoryTransactionsPage {
  private readonly txApi = inject(InventoryTransactionApi);
  private readonly branchApi = inject(BranchApi);
  private readonly inventoryApi = inject(InventoryApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly success = signal<string | null>(null);
  
  protected readonly transactions = signal<InventoryTransactionDto[]>([]);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly ingredients = signal<LookupItem[]>([]);
  
  protected readonly isCreating = signal(false);

  protected readonly form = signal<CreateInventoryTransactionRequestDto>({
    ingredientId: '',
    branchId: this.authStore.branchId() ?? '',
    type: 1, // 1 = Nhập, 2 = Xuất
    quantity: 1,
    unitCost: 0,
    referenceNumber: '',
    notes: ''
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({ next: (branches) => this.branches.set(branches) });
    this.inventoryApi.getIngredientLookups().subscribe({ next: (ingredients) => this.ingredients.set(ingredients) });
    
    this.txApi.getAll().subscribe({
      next: (txs) => {
        this.transactions.set(txs);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Không tải được lịch sử giao dịch.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.isCreating.set(true);
    this.success.set(null);
    this.error.set(null);
    this.form.set({
      ingredientId: '',
      branchId: this.authStore.branchId() ?? '',
      type: 1,
      quantity: 1,
      unitCost: 0,
      referenceNumber: '',
      notes: ''
    });
  }

  protected cancelCreate(): void {
    this.isCreating.set(false);
  }

  protected submit(): void {
    const payload = this.form();
    this.error.set(null);
    this.success.set(null);

    if (!payload.branchId || !payload.ingredientId || payload.quantity <= 0) {
      this.error.set('Vui lòng điền chi nhánh, nguyên liệu và số lượng hợp lệ.');
      return;
    }

    if (payload.type === 1 && payload.unitCost <= 0) {
      this.error.set('Vui lòng nhập đơn giá khi nhập kho để tính chi phí âm.');
      return;
    }

    this.submitting.set(true);
    this.txApi.create(payload).subscribe({
      next: () => {
        this.submitting.set(false);
        this.isCreating.set(false);
        this.success.set('Tạo giao dịch kho thành công.');
        this.load();
      },
      error: (err) => {
        this.submitting.set(false);
        this.error.set(err?.error?.message ?? 'Không thể tạo giao dịch kho.');
      }
    });
  }

  protected displayQuantity(quantity: number): number {
    return Math.abs(quantity);
  }

  protected getTypeName(type: number): string {
    if (type === 1) return 'Nhập kho';
    if (type === 2) return 'Xuất kho';
    return 'Khác';
  }

  protected getTypeClass(type: number): string {
    if (type === 1) return 'badge-import';
    if (type === 2) return 'badge-export';
    return 'badge-other';
  }
}
