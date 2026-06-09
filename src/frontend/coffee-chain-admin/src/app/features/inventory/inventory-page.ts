import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LookupItem } from '../../core/models/common.models';
import { BranchSummary } from '../../core/models/dashboard.models';
import { InventoryItem, UpsertInventoryItemRequest } from '../../core/models/inventory.models';
import { AuthStore } from '../../core/services/auth.store';
import { BranchApi } from '../../core/services/branch.api';
import { InventoryApi } from '../../core/services/inventory.api';

// InventoryPage noi voi API CRUD ton kho va cho phep manager/admin sua theo branch permission.
@Component({
  selector: 'ccm-inventory-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory-page.html',
  styleUrl: './inventory-page.css'
})
export class InventoryPage {
  private readonly inventoryApi = inject(InventoryApi);
  private readonly branchApi = inject(BranchApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly items = signal<InventoryItem[]>([]);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly ingredients = signal<LookupItem[]>([]);
  protected readonly editingId = signal<string | null>(null);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(5);

  protected readonly form = signal<UpsertInventoryItemRequest>({
    ingredientId: null,
    ingredientName: '',
    unit: '',
    reorderLevel: 0,
    branchId: this.authStore.branchId() ?? '',
    inStockQuantity: 0,
    reservedQuantity: 0
  });

  constructor() {
    this.load();
  }

  protected readonly filteredItems = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.items();
    }

    return this.items().filter((item) =>
      [item.ingredientName, item.branchName, item.unit].join(' ').toLowerCase().includes(term));
  });

  protected readonly pagedItems = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredItems().slice(start, start + this.pageSize());
  });

  protected readonly totalPages = computed(() => {
    const total = this.filteredItems().length;
    return Math.max(1, Math.ceil(total / this.pageSize()));
  });

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({ next: (branches) => this.branches.set(branches) });
    this.inventoryApi.getIngredientLookups().subscribe({ next: (ingredients) => this.ingredients.set(ingredients) });
    this.inventoryApi.getAll().subscribe({
      next: (items) => {
        this.items.set(items);
        this.page.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc du lieu ton kho.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.editingId.set(null);
    this.page.set(1);
    this.form.set({
      ingredientId: null,
      ingredientName: '',
      unit: '',
      reorderLevel: 0,
      branchId: this.authStore.branchId() ?? '',
      inStockQuantity: 0,
      reservedQuantity: 0
    });
  }

  protected startEdit(item: InventoryItem): void {
    this.editingId.set(item.id);
    this.page.set(1);
    this.form.set({
      ingredientId: item.ingredientId,
      ingredientName: item.ingredientName,
      unit: item.unit,
      reorderLevel: item.reorderLevel,
      branchId: item.branchId,
      inStockQuantity: item.inStockQuantity,
      reservedQuantity: item.reservedQuantity
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
      ? this.inventoryApi.update(this.editingId()!, payload)
      : this.inventoryApi.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        this.startCreate();
        this.load();
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Khong luu duoc ton kho.');
      }
    });
  }

  protected delete(item: InventoryItem): void {
    if (!confirm(`Xóa ton kho cua ${item.ingredientName} tai ${item.branchName}?`)) {
      return;
    }

    this.inventoryApi.delete(item.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Khong xoa duoc ton kho.')
    });
  }

  protected canWrite(item?: InventoryItem): boolean {
    return this.authStore.can('inventory.write', item?.branchId ?? this.form().branchId);
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

  private validate(payload: UpsertInventoryItemRequest): string | null {
    if (!payload.branchId) {
      return 'Chi nhánh là bắt buộc.';
    }

    if (!payload.ingredientId && (!payload.ingredientName?.trim() || !payload.unit?.trim())) {
      return 'Can chon ingredient co san hoac nhap ten + don vi ingredient moi.';
    }

    return null;
  }
}
